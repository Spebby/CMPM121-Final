using System.Collections.Generic;
using CMPM.AI;
using CMPM.AI.BehaviorTree;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Enemies;
using CMPM.UI;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.Movement {
    public class EnemyController : MonoBehaviour {
        #region Publics
        public Transform target;
        public int speed;
        public Hittable HP;
        [FormerlySerializedAs("healthui")] public HealthBar healthUI;
        public bool dead;

        #region Actions
        readonly Dictionary<EnemyActionTypes, EnemyAction> _actions = new();
        Dictionary<string, int> _effects;
        public bool canBeBuffed = true;
        #endregion
        
        [FormerlySerializedAs("strength_pip")] public GameObject strengthPip;
        List<GameObject> _pips;

        public BehaviorType type;
        public BehaviorTree Behavior;
        #endregion

        void Start() {
            target =  GameManager.Instance.Player.transform;
            HP.OnDeath += Die;
            healthUI.SetHealth(HP);

            GetComponent<Unit>().speed = speed;
            _pips                      = new List<GameObject>();
        }

        void Update() {
            if (GameManager.Instance.State != GameManager.GameState.INWAVE)
                Destroy(gameObject);
            else {
                int str = GetEffect("strength");
                while (str > _pips.Count) {
                    GameObject newPip = Instantiate(strengthPip, transform);
                    newPip.transform.localPosition = new Vector3(-0.4f + _pips.Count * 0.125f, -0.55f, 0);
                    _pips.Add(newPip);
                }

                while (_pips.Count > str) {
                    GameObject pip = _pips[^1];
                    _pips.RemoveAt(_pips.Count - 1);
                    Destroy(pip);
                }


                Behavior?.Run();
            }
        }

        public void AddAction(EnemyActionTypes type, EnemyAction action) {
            action.Enemy  =   this;
            _actions[type] =   action;
        }

        public EnemyAction GetAction(EnemyActionTypes type) {
            return _actions.GetValueOrDefault(type, null);
        }

        public void AddEffect(string name, int stacks) {
            _effects ??= new Dictionary<string, int>();
            _effects.TryAdd(name, 0);

            _effects[name] += stacks;
            if (_effects[name] > 10) _effects[name] = 10;
        }

        public int GetEffect(string name) {
            return _effects?.GetValueOrDefault(name, 0) ?? 0;
        }

        void Die() {
            if (dead) return;
            dead = true;
            EventBus.Instance.DoEnemyDeath(this);
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}