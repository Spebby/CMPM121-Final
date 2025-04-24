using System.Collections.Generic;
using System.Linq;
using CMPM.Spells;
using CMPM.Sprites;
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
        public static readonly GameManager INSTANCE = new();

        public GameObject Player;

        public ProjectileManager ProjectileManager;
        public SpellIconManager SpellIconManager;
        public EnemySpriteManager EnemySpriteManager;
        public PlayerSpriteManager PlayerSpriteManager;
        public RelicIconManager RelicIconManager;

        readonly List<GameObject> _enemies;

        public int EnemyCount => _enemies.Count;

        public void AddEnemy(GameObject enemy) {
            _enemies.Add(enemy);
        }

        public void RemoveEnemy(GameObject enemy) {
            _enemies.Remove(enemy);
        }

        public GameObject GetClosestEnemy(Vector3 point) {
            if (_enemies == null || _enemies.Count == 0) return null;
            if (_enemies.Count == 1) return _enemies[0];
            return _enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude
                                              < (b.transform.position - point).sqrMagnitude
                                          ? a
                                          : b);
        }

        GameManager() {
            _enemies = new List<GameObject>();
        }
    }
}