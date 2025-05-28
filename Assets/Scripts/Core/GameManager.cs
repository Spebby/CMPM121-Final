using System;
using System.Collections.Generic;
using System.Linq;
using CMPM.Spells;
using CMPM.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CMPM.Core {
    public class GameManager {
        public enum GameState {
            PREGAME,
            INWAVE,
            WAVEEND,
            COUNTDOWN,
            GAMEOVER
        }

        public GameState State { get; private set; }
        public static event Action<GameState> OnStateChanged;

        public void SetState(GameState state) {
            State = state;

            switch (state) {
                case GameState.INWAVE:
                    EventBus.Instance.DoWaveStart();
                    break;
                case GameState.WAVEEND:
                    EventBus.Instance.DoWaveEnd();
                    break;
                case GameState.PREGAME:
                case GameState.COUNTDOWN:
                case GameState.GAMEOVER:
                default:
                    break;
            }
            
            OnStateChanged?.Invoke(state);
        }

        public int Countdown;
        public static readonly GameManager Instance = new();

        public GameObject Player;
        public PlayerController PlayerController;

        public ProjectileManager ProjectileManager;
        public SpellIconManager SpellIconManager;
        public EnemySpriteManager EnemySpriteManager;
        public PlayerSpriteManager PlayerSpriteManager;
        public RelicIconManager RelicIconManager;

        readonly List<GameObject> _enemies;
        public int EnemiesLeft;

        public int TotalWaves;
        public int CurrentWave;

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
                Object.Destroy(enemy);
                EnemiesLeft--;
            }
        }

        public GameObject GetClosestEnemy(Vector3 point) {
            lock (ENEMY_LOCK) {
                GameObject closest = null;
                float      minDist = float.MaxValue;

                foreach (GameObject enemy in _enemies) {
                    if (!enemy) continue; // handles destroyed objects

                    Transform t = enemy.transform;
                    if (!t) continue; // handles edge-case "zombie" objects

                    float dist = (t.position - point).sqrMagnitude;
                    if (!(dist < minDist)) continue;
                    minDist = dist;
                    closest = enemy;
                }

                return closest;
            }
        }

        public GameObject GetClosestOtherEnemy(GameObject self) {
            Vector3 point = self.transform.position;
            lock (ENEMY_LOCK) {
                if (_enemies == null || _enemies.Count < 2) return null;
                return _enemies.FindAll((a) => a != self).Aggregate(
                    (a, b) => (a.transform.position - point).sqrMagnitude
                            < (b.transform.position - point).sqrMagnitude
                        ? a
                        : b);
            }
        }

        public List<GameObject> GetEnemiesInRange(Vector3 point, float distance) {
            lock (ENEMY_LOCK) {
                if (_enemies == null || _enemies.Count == 0) return null;
                return _enemies.FindAll((a) => (a.transform.position - point).magnitude <= distance);
            }
        }

        public void SetGameOver() {
            lock (ENEMY_LOCK) {
                // this is terrible
                foreach (GameObject e in _enemies) {
                    Object.Destroy(e);
                }

                _enemies.Clear();
                SetState(GameState.GAMEOVER);
            }
        }

        GameManager() {
            _enemies = new List<GameObject>();
            State    = GameState.PREGAME;
        }
    }
}