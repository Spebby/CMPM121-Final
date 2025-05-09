using System.Collections.Generic;
using System.Linq;
using CMPM.DamageSystem;
using CMPM.Level;
using CMPM.Movement;
using CMPM.Spells;
using CMPM.Sprites;
using UnityEditor;
using UnityEngine;


namespace CMPM.Core {
    public class GameManager {
        public enum GameState {
            PREGAME,
            INWAVE,
            WAVEEND,
            COUNTDOWN,
            GAMEOVER
        }

        public GameState State;

        public int Countdown;
        public static readonly GameManager Instance = new();

        public GameObject Player;

        public ProjectileManager ProjectileManager;
        public SpellIconManager SpellIconManager;
        public EnemySpriteManager EnemySpriteManager;
        public PlayerSpriteManager PlayerSpriteManager;
        public RelicIconManager RelicIconManager;

        List<GameObject> _enemies;
        public int EnemiesLeft;

        public int totalWaves;
        public int currentWave;
        public int MaxModifierCount { get; private set; }

        // slightly unthreadsafe
        public int EnemyCount => _enemies.Count;

        static readonly object ENEMY_LOCK = new(); 
        public void AddEnemy(GameObject enemy) {
            lock (ENEMY_LOCK) {
                _enemies.Add(enemy);
            }
        }

        public void RemoveEnemy(GameObject enemy) {
            lock (ENEMY_LOCK) {
                _enemies.Remove(enemy);
            }
            EnemiesLeft--;
        }

        public GameObject GetClosestEnemy(Vector3 point) {
            lock (ENEMY_LOCK) {
                if (_enemies == null || _enemies.Count == 0) return null;
                if (_enemies.Count == 1) return _enemies[0];
                return _enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude
                                                  < (b.transform.position - point).sqrMagnitude
                                              ? a
                                              : b);
            }
        }

        public void SetGameOver() {
            lock (ENEMY_LOCK) {
                // this is terrible
                foreach (GameObject e in _enemies) {
                    Object.Destroy(e);
                }
                _enemies.Clear();
                State = GameState.GAMEOVER;
            }
        }
        
        GameManager() {
            _enemies = new List<GameObject>();
            State = GameState.PREGAME;
        }
    }
}