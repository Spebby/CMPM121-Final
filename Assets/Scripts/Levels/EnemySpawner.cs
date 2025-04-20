using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class EnemySpawner : MonoBehaviour {
    [FormerlySerializedAs("level_selector")]
    public Image levelSelector;

    public GameObject button;
    public GameObject enemy;
    [FormerlySerializedAs("SpawnPoints")] public SpawnPoint[] spawnPoints;

    public Dictionary<string, Enemy> EnemyTypes;
    public List<Level> levels;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        GameObject selector = Instantiate(button, levelSelector.transform);
        selector.transform.localPosition                        = new Vector3(0, 130);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("Start");
    
    
        LoadEnemiesJson(); //to load the enemies.json
        LoadLevelsJson();  //to load the levels.json
    }

    // Update is called once per frame
    void Update() { }

    //used reading JSON file example from Markus Eger's Lecture 4: Coupling
    void LoadEnemiesJson() {
        EnemyTypes = new Dictionary<string, Enemy>();
        var enemyText = Resources.Load<TextAsset>("enemies"); //variable to obtain the enemies.json

        JToken jo = JToken.Parse(enemyText.text);
        // ReSharper disable once LocalVariableHidesMember
        foreach (JToken enemy in jo) {
            var en = enemy.ToObject<Enemy>(); //to read the enemies type
            EnemyTypes[en.name] = en;
        }
    }

    //used https://stackoverflow.com/questions/11126242/using-jsonconvert-deserializeobject-to-deserialize-json-to-a-c-sharp-poco-class to understand DeserializeObject
    void LoadLevelsJson() {
        var levelText = Resources.Load<TextAsset>("levels"); //to obtain the levels.json
        levels = JsonConvert.DeserializeObject<List<Level>>(levelText.text);
    }

    public void StartLevel(string levelName) {
        levelSelector.gameObject.SetActive(false);

        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        
        //get the level name
        Level currentLevel = levels.Find(level => level.name == levelName);

        //to start the level
        GameManager.Instance.Player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave(currentLevel, 1));
    }

    public void NextWave(Level currentLevel, int wave) {
        //to move to the next wave
        StartCoroutine(SpawnWave(currentLevel, wave));
    }

    IEnumerator SpawnWave(Level level, int wave) {
        GameManager.Instance.State     = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.Countdown = 3;
        for (int i = 3; i > 0; i--) {
            yield return new WaitForSeconds(1);
            GameManager.Instance.Countdown--;
        }

        GameManager.Instance.State = GameManager.GameState.INWAVE;

        //to spawn enemies of the wave using start coroutine.
        foreach (var spawn in level.spawns){
            yield return StartCoroutine(SpawnEnemies(spawn, wave)); 
        }
        
        /*
        //this spawn only zombies
        for (int i = 0; i < 10; ++i) {
            yield return this.SpawnZombie();
        }
        */

        //this waits until all enemies are gone
        yield return new WaitWhile(() => GameManager.Instance.EnemyCount > 0);
        GameManager.Instance.State = GameManager.GameState.WAVEEND;
    }

    //to spawn all enemies of one type
    IEnumerator SpawnEnemies(Spawn spawn, int wave){
        int n = 0;
        int count = RpnEvaluator.Evaluate(spawn.count, new Dictionary<string, int> { { "wave", wave } });   //the amount of enemies
        int delay = spawn.delay;                                                                            //delay between consecutive spawns
        List<int> sequence = spawn.sequence;                                                                //how many should be spawned
        int sequenceIndex = 0;                                                                              //index to traverse the sequence list 

        //this was provided by Markus Eger's Lecture 5: Design Patterns in psudocode
        while (n < count){
            int required = sequence[sequenceIndex];
            for(int i = 0; i < required; i++){
                if(n == count){
                    break;
                }
                SpawnEnemy(spawn, wave);        
                n++;
            }
            yield return new WaitForSeconds(delay);
        }
    }

/*
    IEnumerator SpawnZombie() {
        SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2    offset     = Random.insideUnitCircle * 1.8f;

        Vector3    initialPosition = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject newEnemy        = Instantiate(enemy, initialPosition, Quaternion.identity);

        newEnemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.EnemySpriteManager.Get(0);
        var en = newEnemy.GetComponent<EnemyController>();
        en.Hp    = new Hittable(50, Hittable.Team.MONSTERS, newEnemy);
        en.speed = 10;
        GameManager.Instance.AddEnemy(newEnemy);
        yield return new WaitForSeconds(0.5f);
    }
*/

/*
    void SpawnEnemy(Spawn spawn, int wave) {
        // Where? change to only spawn at eligible spawn points (e.g. only red ones)
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;        
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        // Create Instance
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);
        // Set Parameters; you will need to replace the numbers with the evaluated RPN values
        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
        en.speed = 10;
        GameManager.Instance.AddEnemy(new_enemy);
    }
*/
}

//enemy class
[Serializable]
public class Enemy {
    public string name;
    public int sprite;
    public int hp;
    public int speed;
    public int damage;
}

//level class
[Serializable]
public class Level {
    public string name;
    public int wave;
    public List<Spawn> spawns;
}

[Serializable]
public class Spawn {
    public string enemy;
    public string count;
    public string hp;
    public int delay;
    public List<int> sequence;
    public string location;
}

//reverse polish notation class
//this was provided by Markus Eger's Lecture 5: Design Patterns
public class RpnEvaluator {
    public static int Evaluate(string expression, Dictionary<string, int> variables) {
        var stack = new Stack<int>();
        foreach (string token in expression.Split(" ")) //to split the tokens 
        {
            if (variables.TryGetValue(token, out int variable)) { //if a token is a variable name, push it to the stack
                stack.Push(variable);
            }
            else if (token is "+" or "-" or "*" or "/" or "%") {
                //checks if the token is an operator
                int a = stack.Pop();
                int b = stack.Pop();
                stack.Push(ApplyOperator(token, b, a)); //to apply the operations using a helper function
            }
            else { //checks if it's an integer
                stack.Push(int.Parse(token));
            }
        }

        return stack.Pop();
    }

    //to perform these operations
    //used http://www.math.bas.bg/bantchev/place/rpn/rpn.c%23.html to figure out creating a RPN
    public static int ApplyOperator(string op, int b, int a) {
        int result = 0; //to store the result
        switch (op) {
            case "+":
                result = b + a;
                break;
            case "-":
                result = b - a;
                break;
            case "*":
                result = b * a;
                break;
            case "/" when a == 0: //edge case to prevent dividing by zero?
                return result;
            case "/":
                result = b / a;
                break;
            case "%":
                result = b % a;
                break;
        }

        return result;
    }
}