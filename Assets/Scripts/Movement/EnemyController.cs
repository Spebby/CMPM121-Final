using System.Collections.Generic;
using CMPM.AI;
using CMPM.AI.BehaviourTree;
using CMPM.Core;
using CMPM.Enemies;
using CMPM.UI;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.Movement {
    public class EnemyController : Entity
    {
        #region Publics
        public Transform target;
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
        public string enemyName;
        public BehaviourTree Behaviour;
        public AudioSource audioSource;

        Coroutine sfxCoroutine;

        float volPitch = 1f;

        #endregion

        void Start()
        {
            target = GameManager.Instance.Player.transform;
            HP.OnDeath += Die;
            healthUI.SetHealth(HP);

            unit = GetComponent<Unit>();
            _pips = new List<GameObject>();

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 5f;
            audioSource.maxDistance = 40f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;

            sfxCoroutine = StartCoroutine(PlayEnemySFX());
        }

        private SoundTypeEnemies? GetSoundTypeFromName(string name)
        {
            switch (name.ToLower())
            {
                case "zombie": return SoundTypeEnemies.ZOMBIE;
                case "skeleton": return SoundTypeEnemies.SKELETON;
                case "warlock": return SoundTypeEnemies.WARLOCK;
                case "dryad": return SoundTypeEnemies.DRYAD;
                case "goat": return SoundTypeEnemies.GOAT;
                default: return null;
            }
        }

        System.Collections.IEnumerator PlayEnemySFX()
        {
            while (!dead)
            {
                yield return new WaitForSeconds(Random.Range(7f, 32f));
                var soundType = GetSoundTypeFromName(enemyName);
                if (soundType.HasValue)
                {
                    audioSource.pitch = volPitch;
                    switch (soundType.Value)
                    {
                        case SoundTypeEnemies.ZOMBIE:
                            volPitch = Random.Range(0.5f, 1.5f);
                            break;
                        case SoundTypeEnemies.SKELETON:
                            volPitch = Random.Range(2.0f, 3.0f);
                            break;
                        case SoundTypeEnemies.WARLOCK:
                            volPitch = Random.Range(0.5f, 1.5f);
                            break;
                        case SoundTypeEnemies.DRYAD:
                            volPitch = Random.Range(0.5f, 1.5f);
                            break;
                        case SoundTypeEnemies.GOAT:
                            volPitch = Random.Range(0.5f, 2.5f);
                            break;
                        default:
                            volPitch = Random.Range(0.1f, 3.0f);
                            break;
                    }
                    audioSource.PlayOneShot(EnemiesSoundManager.GetEnemyClip(soundType.Value));
                }
            }
        }

        void Update()
        {
            if (GameManager.Instance.State != GameManager.GameState.INWAVE)
                Destroy(gameObject);
            else
            {
                int str = GetEffect("strength");
                while (str > _pips.Count)
                {
                    GameObject newPip = Instantiate(strengthPip, transform);
                    newPip.transform.localPosition = new Vector3(-0.4f + _pips.Count * 0.125f, -0.55f, 0);
                    _pips.Add(newPip);
                }

                while (_pips.Count > str)
                {
                    GameObject pip = _pips[^1];
                    _pips.RemoveAt(_pips.Count - 1);
                    Destroy(pip);
                }


                Behaviour?.Run();
            }
        }

        public void AddAction(EnemyActionTypes actionType, EnemyAction action)
        {
            action.Enemy = this;
            _actions[actionType] = action;
        }

        public EnemyAction GetAction(EnemyActionTypes actionType)
        {
            return _actions.GetValueOrDefault(actionType, null);
        }

        public void AddEffect(string effectName, int stacks)
        {
            _effects ??= new Dictionary<string, int>();
            _effects.TryAdd(effectName, 0);

            _effects[effectName] += stacks;
            if (_effects[effectName] > 10) _effects[effectName] = 10;
        }

        public int GetEffect(string effectName)
        {
            return _effects?.GetValueOrDefault(effectName, 0) ?? 0;
        }

        void Die()
        {
            if (dead) return;
            dead = true;
            EventBus.Instance.DoEnemyDeath(this);
            GameManager.Instance.RemoveEnemy(gameObject);
        }
        
    }
}
