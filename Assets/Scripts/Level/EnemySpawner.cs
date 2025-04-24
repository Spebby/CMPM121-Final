using System;
using System.Collections;
using System.Collections.Generic;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.Structures;
using CMPM.UI;
using CMPM.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace CMPM.Level {
    public class EnemySpawner : MonoBehaviour {
        public GameObject button;
        public GameObject enemy;
        [FormerlySerializedAs("SpawnPoints")] public SpawnPoint[] spawnPoints;

        public Hashtable<string, Enemy> enemyTypes;
        public List<Level> levels;

        [Header("UI")]
        [SerializeField] GameObject levelSelector;
        
        void Awake() {
            // This is ass


            LoadEnemiesJson(Resources.Load<TextAsset>("enemies"));
            LoadLevelsJson(Resources.Load<TextAsset>("levels"), enemyTypes);

            foreach (Level level in levels) {
                GameObject selector = Instantiate(button, levelSelector.transform);
                selector.transform.SetParent(levelSelector.transform);
                MenuSelectorController msController = selector.GetComponent<MenuSelectorController>();
                msController.spawner = this;
                msController.SetLevel(level.name);
            }
        }

        void LoadEnemiesJson(in TextAsset enemyText) {
            enemyTypes = new Hashtable<string, Enemy>();
            
            foreach (JToken _ in JToken.Parse(enemyText.text)) {
                Enemy e = _.ToObject<Enemy>();
                enemyTypes[e.name] = e;
            }
        }

        void LoadLevelsJson(in TextAsset levelText, in Hashtable<string, Enemy> enemies) {
            levels = JsonConvert.DeserializeObject<List<Level>>(levelText.text);
            // There's a more suave way to do this w/ JsonConverters but I'd rather not get caught up in that rn.
            foreach (Level level in levels) {
                for (int i = 0; i < level.spawns.Count; i++) {
                    Spawn spawn = level.spawns[i];
                    
                    spawn.HPFormula = string.IsNullOrEmpty(spawn.HPFormula)
                        ? Convert.ToString(enemies[spawn.enemy].baseHP)
                        : spawn.HPFormula;
                    
                    spawn.damageFormula = string.IsNullOrEmpty(spawn.damageFormula)
                        ? Convert.ToString(enemies[spawn.enemy].damage)
                        : spawn.damageFormula;

                    spawn.speedFormula = string.IsNullOrEmpty(spawn.speedFormula)
                        ? Convert.ToString(enemies[spawn.enemy].damage)
                        : spawn.speedFormula;
                    
                    
                    
                    level.spawns[i] = spawn;
                }
            }
        }

        public void StartLevel(string levelName) {
            levelSelector.gameObject.SetActive(false);

            // this is not nice: we should not have to be required to tell the player directly that the level is starting

            Level currentLevel = levels.Find(level => level.name == levelName);

            //to start the level
            GameManager.INSTANCE.Player.GetComponent<PlayerController>().StartLevel();
            StartCoroutine(SpawnWave(currentLevel, 1));
        }

        public void NextWave(Level currentLevel, int wave) {
            //to move to the next wave
            StartCoroutine(SpawnWave(currentLevel, wave));
        }

        IEnumerator SpawnWave(Level level, int wave) {
            GameManager.INSTANCE.State     = GameManager.GameState.COUNTDOWN;
            GameManager.INSTANCE.Countdown = 3;
            for (int i = 3; i > 0; i--) {
                yield return new WaitForSeconds(1);
                GameManager.INSTANCE.Countdown--;
            }

            GameManager.INSTANCE.State = GameManager.GameState.INWAVE;

            foreach (Spawn spawn in level.spawns) {
                yield return StartCoroutine(SpawnEnemies(spawn, wave));
            }

            yield return new WaitWhile(() => GameManager.INSTANCE.EnemyCount > 0);
            GameManager.INSTANCE.State = GameManager.GameState.WAVEEND;
        }

        //to spawn all enemies of one type
        IEnumerator SpawnEnemies(Spawn spawn, int wave) {
            int n = 0;
            int count = RPN.Evaluate(spawn.count, new Hashtable<string, int> { { "wave", wave } }); //the amount of enemies
            int       delay         = spawn.delay; //delay between consecutive spawns
            List<int> sequence      = spawn.sequence; //how many should be spawned
            int       sequenceIndex = 0; //index to traverse the sequence list 

            //this was provided by Markus Eger's Lecture 5: Design Patterns in psudocode
            while (n < count) {
                int required = sequence[sequenceIndex];
                for (int i = 0; i < required; i++) {
                    if (n == count) {
                        break;
                    }

                    SpawnEnemy(spawn, wave);
                    n++;
                }

                yield return new WaitForSeconds(delay);
            }
        }

        IEnumerator SpawnZombie() {
            SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector2    offset     = Random.insideUnitCircle * 1.8f;

            Vector3    initialPosition = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject newEnemy        = Instantiate(enemy, initialPosition, Quaternion.identity);

            newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.INSTANCE.EnemySpriteManager.Get(0);
            EnemyController en = newEnemy.GetComponent<EnemyController>();
            en.HP    = new Hittable(50, Hittable.Team.MONSTERS, newEnemy);
            en.speed = 10;
            GameManager.INSTANCE.AddEnemy(newEnemy);
            yield return new WaitForSeconds(0.5f);
        }

        void SpawnEnemy(in Spawn spawn, in int wave) {
            // Where? change to only spawn at eligible spawn points (e.g. only red ones)
            SpawnPoint spawnPoint      = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector2    offset          = Random.insideUnitCircle * 1.8f;
            Vector3    initialPosition = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);
            // Create Instance
            GameObject newEnemy = Instantiate(enemy, initialPosition, Quaternion.identity);
            // Set Parameters; you will need to replace the numbers with the evaluated RPN values
            newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.INSTANCE.EnemySpriteManager.Get(0);
            EnemyController en = newEnemy.GetComponent<EnemyController>();
            en.HP    = new Hittable(50, Hittable.Team.MONSTERS, newEnemy);
            en.speed = 10;
            GameManager.INSTANCE.AddEnemy(newEnemy);
        }
    }

    [Serializable]
    public struct Enemy {
        public string name;
        public int sprite;
        [JsonProperty("hp")]
        public int baseHP;
        public int speed;
        public int damage;
    }

    [Serializable]
    public class Level {
        public string name;
        public int waves;
        public List<Spawn> spawns;
    }

    // TODO: replace enemy ref w/ JSON Parser
    [Serializable]
    public struct Spawn {
        public string enemy;
        public string count;
        [JsonProperty("hp")]
        public string HPFormula;
        [JsonProperty("speed")]
        public string speedFormula;
        [JsonProperty("damage")]
        public string damageFormula;
        [Tooltip("In ms.")] public int delay;
        public List<int> sequence;
        public string location;
    }
}