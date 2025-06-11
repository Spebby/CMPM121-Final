using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CMPM.AI;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Enemies;
using CMPM.MapGenerator;
using CMPM.Movement;
using CMPM.Utils;
using CMPM.Utils.LevelParsing;
using CMPM.Utils.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
// This is the most imports I've ever had in my life.


namespace CMPM.Level {
    public class EnemySpawner : MonoBehaviour {
        [FormerlySerializedAs("enemy")] public GameObject enemyPrefab;
        public static EnemySpawner Instance { get; private set; }
        
        
        // We may be able to get away w/ making this a static member for easier usage for parser,
        // but there's not a (ton) of reason to expose it publicly to begin with.
        SerializedDictionary<string, Enemy> _enemyTypes;
        List<Difficulty> _difficulties;

        [Header("UI")] [SerializeField] GameObject levelSelector;

        #region Privates
        Room _currentRoom;
        int _currentFloor;
        int _remainingSpawns;
        #endregion

        void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject); // Optional: clean up duplicates
                return;
            }
            Instance = this;
            
            LoadEnemiesJson(Resources.Load<TextAsset>("enemies"));
            //LoadDifficultiesJson(Resources.Load<TextAsset>("levels"), enemyTypes);

            /*
            foreach (Difficulty difficulty in _difficulties) {
                GameObject selector = Instantiate(button, levelSelector.transform);
                selector.transform.SetParent(levelSelector.transform);
                MenuSelectorController msController = selector.GetComponent<MenuSelectorController>();
                msController.spawner = this;
                msController.SetLevel(difficulty.name);
            }
            */
        }

        public void EndWave() {
            _currentRoom.UnlockDoors();
        }

        // It may be good to also make this async at some point. I don't see any reason *why* it has to sync w/
        // game manager, especially as this function gets more complex
        public void SpawnEnemies(in Room room) {
            _currentRoom  = room;
            _currentFloor = GameManager.Instance.CurrentFloor;

            RoomSpawn roomSpawns = room.spawns;
            
            if (roomSpawns.spawns.Length == 0) return;
            GameManager.Instance.SetState(GameManager.GameState.INCOMBAT);
            
            // Precompute valid enemies
            Dictionary<SpawnPoint.SpawnName, (Spawn[], int[])> table = new();
            foreach (SpawnPoint.SpawnName spawnName in Enum.GetValues(typeof(SpawnPoint.SpawnName))) {
                List<Spawn> temp = new();
                List<int>  temp2 = new();
                foreach (Spawn spawn in roomSpawns.spawns) {
                    if (spawn.MinFloor > _currentFloor) continue;
                    bool validLocation = spawn.Locations.Contains(spawnName) || spawn.Locations.Contains(SpawnPoint.SpawnName.RANDOM);
                    if (!validLocation) continue;
                    temp.Add(spawn);
                    temp2.Add(spawn.Weight);
                }

                table[spawnName] = (temp.ToArray(), temp2.ToArray());
            }

            // One enemy per point
            foreach (SpawnPoint point in room.spawnpoints) {
                (Spawn[], int[]) valid = table[point.Kind];
                if (valid.Item1.Length == 0) continue;

                Spawn spawn = valid.Item1[0];
                { // Pick an enemy to spawn at this point w/ weights
                    int   weightSum  = valid.Item2.Sum();
                    int   pick       = Random.Range(0, weightSum);
                    float cumulative = 0f;

                    for (int i = 0; i < valid.Item1.Length; ++i) {
                        cumulative += valid.Item2[i];
                        if (!(pick <= cumulative)) continue;
                        spawn = valid.Item1[i];
                    }
                }

                Enemy enemy = _enemyTypes[spawn.EnemyName];
                (int HP, int Damage, int Speed) ep = (
                    HP: (int)spawn.HPFormula.Evaluate(new SerializedDictionary<string, float> {
                        { "floor", _currentFloor }, { "base", enemy.baseHP }
                    }),
                    Damage: spawn.DamageFormula.Evaluate(new SerializedDictionary<string, int> {
                        { "floor", _currentFloor }, { "base", enemy.damage }
                    }),
                    Speed: spawn.SpeedFormula.Evaluate(new SerializedDictionary<string, int> {
                        { "floor", _currentFloor }, { "base", enemy.speed }
                    }));
                
                SpawnEnemy(enemy, ep, point.transform.position);
            }
        }

        void SpawnEnemy(in Enemy enemy, in (int HP, int Damage, int Speed) packet, in Vector3 point) {
            Vector2    offset = Random.insideUnitCircle * 1.8f;
            Vector3    initialPosition = point + new Vector3(offset.x, offset.y, 0);
            GameObject newEnemy        = Object.Instantiate(enemyPrefab, initialPosition, Quaternion.identity);

            newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySpriteManager.Get(enemy.sprite);
            EnemyController en = newEnemy.GetComponent<EnemyController>();

            switch (enemy.type) {
                case BehaviourType.Support:
                    en.AddAction(EnemyActionTypes.Attack, new EnemyAttack(enemy.cooldown, enemy.range, packet.Damage, enemy.strengthFactor));
                    en.AddAction(EnemyActionTypes.Heal, new EnemyHeal(10, 5, 15));
                    en.AddAction(EnemyActionTypes.Buff, new EnemyBuff(8, 5, 3, 8));
                    en.AddAction(EnemyActionTypes.Permabuff, new EnemyBuff(20, 5, 1));
                    en.AddEffect("noheal", 1);
                    break;
                case BehaviourType.Swarmer:
                    en.AddAction(EnemyActionTypes.Attack, new EnemyAttack(enemy.cooldown, enemy.range, enemy.damage, enemy.strengthFactor));
                    break;
                default:
                    throw new NotImplementedException($"Behaviour Type {enemy.type} not implemented!");
            }

            en.type = enemy.type;
            en.Behaviour = BehaviourBuilder.MakeTree(en);
            en.HP        = new Hittable(packet.HP, Hittable.Team.MONSTERS, en);
            en.ModifySpeed(packet.Speed);

            // I don't have the time to refactor the enemy rn to make the damage amount be different
            GameManager.Instance.AddEnemy(newEnemy);
        }
        
        void LoadEnemiesJson(in TextAsset enemyText) {
            _enemyTypes = new SerializedDictionary<string, Enemy>();

            foreach (JToken _ in JToken.Parse(enemyText.text)) {
                Enemy e = _.ToObject<Enemy>();
                _enemyTypes[e.name] = e;
            }
        }

        // I wrote some JsonConverters to make the parsing logic less cluttered.
        // I may change more but for the moment this is acceptable.
        void LoadLevelsJson(in TextAsset levelText, in SerializedDictionary<string, Enemy> enemies) {
            // Set up a custom JsonConverter that includes the enemies dictionary
            JsonSerializerSettings settings = new() {
                Converters = new List<JsonConverter> {
                    new SpawnEnemyParser(enemies)
                }
            };

            /*
            difficulties = JsonConvert.DeserializeObject<List<RoomSpawn>>(levelText.text, settings);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator <-- Resharper is wrong
            foreach (RoomSpawn level in difficulties) {
                foreach (ref Spawn spawn in level.spawns.AsSpan()) {
                    ref Enemy fallback = ref spawn.enemy;
                    FormulaFallback(ref spawn.HPFormula, fallback.baseHP);
                    FormulaFallback(ref spawn.DamageFormula, fallback.damage);
                    FormulaFallback(ref spawn.SpeedFormula, fallback.speed);
                    spawn.sequence ??= new[] { 1 };
                }
            }
            */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void FormulaFallback(ref RPNString str, int fallback) {
            str = string.IsNullOrEmpty(str) ? new RPNString(Convert.ToString(fallback)) : str;
        }
    }
    
    [Serializable]
    public struct Enemy {
        public string name;
        public int sprite;
        [JsonProperty("hp")] public int baseHP;
        public int speed;
        public int damage;
        public float cooldown;
        public float range;
        public float strengthFactor;
        [JsonProperty("behaviour")] public BehaviourType type;
    }

    // TODO: replace enemy ref w/ JSON Parser

    struct Difficulty {
        public string name;
        
    }
}
