using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMPM.AI;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Enemies;
using CMPM.Movement;
using CMPM.UI;
using CMPM.Utils;
using CMPM.Utils.LevelParsing;
using CMPM.Utils.Structures;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
// This is the most imports I've ever had in my life.


namespace CMPM.Level {
    public class EnemySpawner : MonoBehaviour {
        public GameObject button;
        [FormerlySerializedAs("enemy")] public GameObject enemyPrefab;
        public SpawnPoint[] spawnPoints;

        // We may be able to get away w/ making this a static member for easier usage for parser,
        // but there's not a (ton) of reason to expose it publicly to begin with.
        public SerializedDictionary<string, EnemyData> enemyTypes;
        List<Level> _levels;

        [Header("UI")] [SerializeField] GameObject levelSelector;

        #region Privates
        Level _currentLevel;
        int _currentWave;
        int _remainingSpawns;
        #endregion

        void Awake() {
            LoadEnemiesJson(Resources.Load<TextAsset>("enemies"));
            LoadLevelsJson(Resources.Load<TextAsset>("levels"), enemyTypes);

            // TODO: In the future I'd like a little more artistic control
            foreach (Level level in _levels) {
                GameObject selector = Instantiate(button, levelSelector.transform);
                selector.transform.SetParent(levelSelector.transform);
                MenuSelectorController msController = selector.GetComponent<MenuSelectorController>();
                msController.spawner = this;
                msController.SetLevel(level.Name);
            }
        }

        public void StartLevel(string levelName) {
            levelSelector.gameObject.SetActive(false);

            // this is not nice: we should not have to be required to tell the player directly that the level is starting

            Level currentLevel = _levels.Find(level => level.Name == levelName);
            GameManager.Instance.TotalWaves  = currentLevel.Waves;
            GameManager.Instance.CurrentWave = 1;

            //to start the level
            GameManager.Instance.PlayerController.StartLevel();
            StartCoroutine(SpawnWave(currentLevel, 1));
        }

        public void NextWave() {
            StartCoroutine(SpawnWave(_currentLevel, ++_currentWave));
        }

        // It may be good to also make this async at some point. I don't see any reason *why* it has to sync w/
        // game manager, especially as this function gets more complex
        IEnumerator SpawnWave(Level level, int wave) {
            _currentLevel                    = level;
            _currentWave                     = wave;
            GameManager.Instance.CurrentWave = _currentWave;

            GameManager.Instance.SetState(GameManager.GameState.COUNTDOWN);
            GameManager.Instance.Countdown = 3;

            {
                SerializedDictionary<string, int> table      = new() { { "wave", wave } };
                Span<Spawn>                       span       = level.Spawns.AsSpan();
                int                               maxEnemies = 0;
                for (int i = 0; i < span.Length; i++) {
                    maxEnemies += span[i].Count.Evaluate(table);
                }

                GameManager.Instance.EnemiesLeft = maxEnemies;
            }

            for (int i = 3; i > 0; i--) {
                yield return new WaitForSeconds(1);
                GameManager.Instance.Countdown--;
            }

            GameManager.Instance.SetState(GameManager.GameState.INWAVE);


            // Definition of Embarrassingly Parallel
            foreach (Spawn spawn in level.Spawns) {
                _ = SpawnEnemies(spawn, wave);
            }

            yield return new WaitWhile(()  => GameManager.Instance.EnemiesLeft > 0);
            if (GameManager.Instance.State != GameManager.GameState.GAMEOVER) {
                GameManager.Instance.SetState(GameManager.GameState.WAVEEND);
            }
        }

        // TODO: Convert to RPNStrings
        async Task SpawnEnemies(Spawn spawn, int wave) {
            int   n             = 0;
            int   count         = spawn.Count.Evaluate(new SerializedDictionary<string, int> { { "wave", wave } });
            int   delay         = spawn.Delay;
            int[] sequence      = spawn.Sequence;
            int   sequenceIndex = 0;

            SpawnPoint[] validSpawns = spawnPoints;
            if (spawn.Location != SpawnPoint.SpawnName.RANDOM) {
                validSpawns = spawnPoints.Where(point => point.Kind == spawn.Location).ToArray();
            }

            // Fallback
            if (validSpawns.Length == 0) {
                Debug.LogWarning("No valid spawn points found. Using fallback.");
                validSpawns = new[] { spawnPoints[0] };
            }

            EnemyPacket ep = new() {
                HP = spawn.HPFormula.Evaluate(new SerializedDictionary<string, int>()
                                                  { { "wave", wave }, { "base", spawn.EnemyData.HP } }),
                Damage = spawn.DamageFormula.Evaluate(new SerializedDictionary<string, int>()
                                                          { { "wave", wave }, { "base", spawn.EnemyData.Damage } }),
                Speed = spawn.SpeedFormula.Evaluate(new SerializedDictionary<string, int>()
                                                        { { "wave", wave }, { "base", spawn.EnemyData.Speed } })
            };

            // Provided by Markus Eger's Lecture 5: Design Patterns in pseudocode
            while (n < count) {
                int required = sequence![sequenceIndex];
                // ++x does have a meaningful difference against x++ in this case. Prefix means that we increment then get, suffix means get then increment.
                sequenceIndex = ++sequenceIndex % sequence.Length;
                for (int i = 0; i < required && n < count; i++, n++) {
                    SpawnEnemy(spawn, ep, validSpawns);
                }

                await Task.Delay(delay);
            }
        }

        // TOOD: Get rid of "packet". I don't remember why I seperated things this way and ideally we don't have
        // multiple structs that provide values for the same thing (Ex packet and spawn both define a "HP"
        
        // I think packet was supposed to "simplify" the evaluation process by pre-computing all the RPN stuff.
        // However there's legitimate reason for someone to want to re-evaluate RPN everytime it's accessed, so
        // we'll want to do away with pre-computing it methinks.
        void SpawnEnemy(in Spawn spawn, in EnemyPacket packet, in SpawnPoint[] points) {
            SpawnPoint p      = points[Random.Range(0, points.Length)];
            Vector2    offset = Random.insideUnitCircle * 1.8f;

            Vector3    initialPosition = p.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject newEnemy        = Instantiate(enemyPrefab, initialPosition, Quaternion.identity);

            EnemyData subject = spawn.EnemyData;
            newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySpriteManager.Get(subject.Sprite);
            EnemyController en = newEnemy.GetComponent<EnemyController>();

            switch (subject.Type) {
                case BehaviourType.Support:
                    en.AddAction(EnemyActionTypes.Attack, new EnemyAttack(subject.Cooldown, subject.Range, packet.Damage, subject.StrengthFactor));
                    en.AddAction(EnemyActionTypes.Heal, new EnemyHeal(10, 5, 15));
                    en.AddAction(EnemyActionTypes.Buff, new EnemyBuff(8, 5, 3, 8));
                    en.AddAction(EnemyActionTypes.Permabuff, new EnemyBuff(20, 5, 1));
                    en.AddEffect("noheal", 1);
                    break;
                case BehaviourType.Swarmer:
                    en.AddAction(EnemyActionTypes.Attack, new EnemyAttack(subject.Cooldown, subject.Range, subject.Damage, subject.StrengthFactor));
                    break;
                default:
                    throw new NotImplementedException($"Behaviour Type {subject.Type} not implemented!");
            }

            en.type = subject.Type;
            en.Behaviour = BehaviourBuilder.MakeTree(en);
            en.HP        = new Hittable(packet.HP, Hittable.Team.MONSTERS, en);
            en.ModifySpeed(packet.Speed);

            // I don't have the time to refactor the enemy rn to make the damage amount be different
            GameManager.Instance.AddEnemy(newEnemy);
        }
        
        
        #region JSON
        void LoadEnemiesJson(in TextAsset enemyText) {
            enemyTypes = new SerializedDictionary<string, EnemyData>();

            foreach (JToken _ in JToken.Parse(enemyText.text)) {
                EnemyData e = _.ToObject<EnemyData>();
                enemyTypes[e.Name] = e;
            }
        }

        // I wrote some JsonConverters to make the parsing logic less cluttered.
        // I may change more but for the moment this is acceptable.
        void LoadLevelsJson(in TextAsset levelText, in SerializedDictionary<string, EnemyData> enemies) {

        }
    }
    
    //[JsonConverter(typeof(EnemyDataParser))]
    public readonly struct EnemyData {
        public readonly string Name;
        public readonly int Sprite;
        public readonly int HP;
        public readonly int Speed;
        public readonly int Damage;
        public readonly float Cooldown;
        public readonly float Range;
        public readonly float StrengthFactor;
        public readonly BehaviourType Type;
        public EnemyData(string name, int sprite, int hp, int speed, int damage, float cooldown, float range, float strengthFactor, BehaviourType type) {
            Name           = name;
            Sprite         = sprite;
            HP             = hp;
            Speed          = speed;
            Damage         = damage;
            Cooldown       = cooldown;
            Range          = range;
            StrengthFactor = strengthFactor;
            Type      = type;
        }
    }

    // TODO: replace enemy ref w/ JSON Parser
    //[JsonConverter(typeof(SpawnDataParser))]
    internal readonly struct Spawn {
        public readonly EnemyData EnemyData;
        public readonly RPNString Count;
        public readonly RPNString HPFormula;
        public readonly RPNString SpeedFormula;
        public readonly RPNString DamageFormula;
        public readonly int Delay;
        [CanBeNull] public readonly int[] Sequence;
        public readonly SpawnPoint.SpawnName Location;
    }

    [Serializable]
    internal readonly struct Level {
        public readonly string Name;
        public readonly int Waves;
        public readonly Spawn[] Spawns;
    }

    struct EnemyPacket {
        public int HP;
        public int Damage;
        public int Speed;
    }
    #endregion
}