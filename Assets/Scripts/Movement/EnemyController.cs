using System.Collections.Generic;
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
        public string monster;
        public Transform target;
        public int speed;
        public Hittable HP;
        [FormerlySerializedAs("healthui")] public HealthBar healthUI;
        public bool dead;

        public Dictionary<string, EnemyAction> Actions;
        public Dictionary<string, int> Effects;
        [FormerlySerializedAs("strength_pip")] public GameObject strengthPip;
        List<GameObject> _pips;

        public BehaviorTree Behavior;
        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            target     =  GameManager.Instance.Player.transform;
            HP.OnDeath += Die;
            healthUI.SetHealth(HP);

            GetComponent<Unit>().speed = speed;
            _pips                      = new List<GameObject>();
        }

        // Update is called once per frame
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

        public void AddAction(string name, EnemyAction action) {
            Actions       ??= new Dictionary<string, EnemyAction>();
            action.Enemy  =   this;
            Actions[name] =   action;
        }

        public EnemyAction GetAction(string name) {
            return Actions.GetValueOrDefault(name, null);
        }

        public void AddEffect(string name, int stacks) {
            Effects ??= new Dictionary<string, int>();
            Effects.TryAdd(name, 0);

            Effects[name] += stacks;
            if (Effects[name] > 10) Effects[name] = 10;
        }

        public int GetEffect(string name) {
            return Effects?.GetValueOrDefault(name, 0) ?? 0;
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