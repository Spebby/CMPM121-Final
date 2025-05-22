using System.Collections.Generic;
using CMPM.AI;
using CMPM.AI.BehaviourTree;
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

        public BehaviourType type;
        public BehaviourTree Behaviour;
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


                Behaviour?.Run();
            }
        }

        public void AddAction(EnemyActionTypes actionType, EnemyAction action) {
            action.Enemy  =   this;
            _actions[actionType] =   action;
        }

        public EnemyAction GetAction(EnemyActionTypes actionType) {
            return _actions.GetValueOrDefault(actionType, null);
        }

        public void AddEffect(string effectName, int stacks) {
            _effects ??= new Dictionary<string, int>();
            _effects.TryAdd(effectName, 0);

            _effects[effectName] += stacks;
            if (_effects[effectName] > 10) _effects[effectName] = 10;
        }

        public int GetEffect(string effectName) {
            return _effects?.GetValueOrDefault(effectName, 0) ?? 0;
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
