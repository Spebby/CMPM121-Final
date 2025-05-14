using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.UI;
using CMPM.Utils;
using CMPM.Utils.SpawningParsers;
using CMPM.Utils.Structures;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


// This is the most imports I've ever had in my life.


namespace CMPM.Level {
    public class EnemySpawner : MonoBehaviour {
        public GameObject button;
        public GameObject enemy;
        public SpawnPoint[] spawnPoints;

        // We may be able to get away w/ making this a static member for easier usage for parser,
        // but there's not a (ton) of reason to expose it publicly to begin with.
        public SerializedDictionary<string, Enemy> enemyTypes;
        public List<Level> levels;

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
            foreach (Level level in levels) {
                GameObject selector = Instantiate(button, levelSelector.transform);
                selector.transform.SetParent(levelSelector.transform);
                MenuSelectorController msController = selector.GetComponent<MenuSelectorController>();
                msController.spawner = this;
                msController.SetLevel(level.name);
            }
        }

        void LoadEnemiesJson(in TextAsset enemyText) {
            enemyTypes = new SerializedDictionary<string, Enemy>();

            foreach (JToken _ in JToken.Parse(enemyText.text)) {
                Enemy e = _.ToObject<Enemy>();
                enemyTypes[e.name] = e;
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

            levels = JsonConvert.DeserializeObject<List<Level>>(levelText.text, settings);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator <-- Resharper is wrong
            foreach (Level level in levels) {
                foreach (ref Spawn spawn in level.spawns.AsSpan()) {
                    ref Enemy fallback = ref spawn.enemy;
                    FormulaFallback(ref spawn.HPFormula, fallback.baseHP);
                    FormulaFallback(ref spawn.DamageFormula, fallback.damage);
                    FormulaFallback(ref spawn.SpeedFormula, fallback.speed);
                    spawn.sequence ??= new[] { 1 };
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void FormulaFallback(ref RPNString str, int fallback) {
            str = string.IsNullOrEmpty(str) ? new RPNString(Convert.ToString(fallback)) : str;
        }

        public void StartLevel(string levelName) {
            levelSelector.gameObject.SetActive(false);

            // this is not nice: we should not have to be required to tell the player directly that the level is starting

            Level currentLevel = levels.Find(level => level.name == levelName);
            GameManager.Instance.TotalWaves  = currentLevel.waves;
            GameManager.Instance.CurrentWave = 1;

            //to start the level
            GameManager.Instance.Player.GetComponent<PlayerController>().StartLevel();
            StartCoroutine(SpawnWave(currentLevel, 1));
        }

        public void NextWave(Level currentLevel, int wave) {
            //to move to the next wave
            StartCoroutine(SpawnWave(currentLevel, wave));
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
                Span<Spawn>                       span       = level.spawns.AsSpan();
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
            foreach (Spawn spawn in level.spawns) {
                _ = SpawnEnemies(spawn, wave);
            }

            yield return new WaitWhile(() => GameManager.Instance.EnemiesLeft > 0);
            if (GameManager.Instance.State != GameManager.GameState.GAMEOVER) {
                GameManager.Instance.SetState(GameManager.GameState.WAVEEND);
            }
        }

        // TODO: Convert to RPNStrings
        async Task SpawnEnemies(Spawn spawn, int wave) {
            int   n             = 0;
            int   count         = spawn.Count.Evaluate(new SerializedDictionary<string, int> { { "wave", wave } });
            int   delay         = spawn.delay;
            int[] sequence      = spawn.sequence;
            int   sequenceIndex = 0;

            SpawnPoint[] validSpawns = spawnPoints;
            if (spawn.location != SpawnPoint.SpawnName.RANDOM) {
                validSpawns = spawnPoints.Where(point => point.kind == spawn.location).ToArray();
            }

            EnemyPacket ep = new() {
                HP = spawn.HPFormula.Evaluate(new SerializedDictionary<string, int>()
                                                  { { "wave", wave }, { "base", spawn.enemy.baseHP } }),
                Damage = spawn.DamageFormula.Evaluate(new SerializedDictionary<string, int>()
                                                          { { "wave", wave }, { "base", spawn.enemy.damage } }),
                Speed = spawn.SpeedFormula.Evaluate(new SerializedDictionary<string, int>()
                                                        { { "wave", wave }, { "base", spawn.enemy.speed } })
            };

            //this was provided by Markus Eger's Lecture 5: Design Patterns in pseudocode
            while (n < count) {
                int required = sequence![sequenceIndex];
                sequenceIndex = ++sequenceIndex % sequence.Length;
                for (int i = 0; i < required; i++) {
                    if (n == count) {
                        break;
                    }

                    SpawnEnemy(spawn, ep, validSpawns);
                    n++;
                }

                await Task.Delay(delay);
            }
        }

        void SpawnEnemy(in Spawn spawn, in EnemyPacket packet, in SpawnPoint[] points) {
            SpawnPoint p      = points[Random.Range(0, points.Length)];
            Vector2    offset = Random.insideUnitCircle * 1.8f;

            Vector3    initialPosition = p.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject newEnemy        = Instantiate(enemy, initialPosition, Quaternion.identity);

            newEnemy.GetComponent<SpriteRenderer>().sprite =
                GameManager.Instance.EnemySpriteManager.Get(spawn.enemy.sprite);
            EnemyController en = newEnemy.GetComponent<EnemyController>();
            en.HP    = new Hittable(packet.HP, Hittable.Team.MONSTERS, newEnemy);
            en.speed = packet.Speed;

            // I don't have the time to refactor the enemy rn to make the damage amount be different
            GameManager.Instance.AddEnemy(newEnemy);
        }
    }

    [Serializable]
    public struct Enemy {
        public string name;
        public int sprite;
        [JsonProperty("hp")] public int baseHP;
        public int speed;
        public int damage;
    }

    [Serializable]
    public class Level {
        public string name;
        public int waves;
        public Spawn[] spawns;
    }

    // TODO: replace enemy ref w/ JSON Parser
    [Serializable]
    public struct Spawn {
        //[JsonConverter(typeof(SpawnEnemyParser))] <-- this is only valid syntax when it's a parameterless constructor
        public Enemy enemy;

        [JsonConverter(typeof(RPNStringParser))] [JsonProperty("count")]
        public RPNString Count;

        [JsonConverter(typeof(RPNStringParser))] [JsonProperty("hp")]
        public RPNString HPFormula;

        [JsonConverter(typeof(RPNStringParser))] [JsonProperty("speed")]
        public RPNString SpeedFormula;

        [JsonConverter(typeof(RPNStringParser))] [JsonProperty("damage")]
        public RPNString DamageFormula;

        [JsonConverter(typeof(SecondsParser))] [Tooltip("In ms.")]
        public int delay;

        [CanBeNull] public int[] sequence;

        [JsonConverter(typeof(SpawnLocationParser))]
        public SpawnPoint.SpawnName location;
    }

    struct EnemyPacket {
        public int HP;
        public int Damage;
        public int Speed;
    }
}