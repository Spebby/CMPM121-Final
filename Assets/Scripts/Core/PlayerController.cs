using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.Spells;
using CMPM.UI;
using CMPM.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CMPM.Core {
    public class PlayerController : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        public readonly Hittable HP;
        SpellCaster _spellCaster;
        [SerializeField] int speed;
        [SerializeField] Unit unit;

        #region UI
        // ReSharper disable once StringLiteralTypo
        [SerializeField, FormerlySerializedAs("healthui")] HealthBar healthUI;
        // ReSharper disable once StringLiteralTypo
        [SerializeField, FormerlySerializedAs("manaui")] ManaBar manaUI;
        // ReSharper disable once StringLiteralTypo
        [SerializeField, FormerlySerializedAs("spellui")] SpellUI spellUI;
        #endregion
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            unit                        = GetComponent<Unit>();
            GameManager.Instance.Player = gameObject;
        }

        public void StartLevel() {
            _spellCaster = new SpellCaster(125, 8, 1, Hittable.Team.PLAYER);
            StartCoroutine(_spellCaster.ManaRegeneration());

            int wave = GameManager.Instance.currentWave;
            SetPlayerScale(wave);
            
            HP.ResetHealth();
            HP.OnDeath += Die;
            HP.team    =  Hittable.Team.PLAYER;

            // tell UI elements what to show
            healthUI.SetHealth(HP);
            manaUI.SetSpellCaster(_spellCaster);
            spellUI.SetSpell(_spellCaster.Spell);
        }

        public void SetPlayerScale(int wave) {
            HP.SetMaxHp(RPN.Evaluate("95 wave 5 * +", new SerializedDictionary<string, int> { { "wave", wave } }));
            _spellCaster.MaxMana    = RPN.Evaluate("90 wave 10 * +", new SerializedDictionary<string, int> { { "wave", wave } });
            _spellCaster.ManaReg    = RPN.Evaluate("10 wave +", new SerializedDictionary<string, int> { { "wave", wave } });
            _spellCaster.SpellPower = RPN.Evaluate("wave 10 *", new SerializedDictionary<string, int> { { "wave", wave } });
            speed                   = 5;
        }
        
        void OnAttack(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
            Vector2 mouseScreen = Mouse.current.position.value;
            Vector3 mouseWorld  = Camera.main!.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0;
            StartCoroutine(_spellCaster.Cast(transform.position, mouseWorld));
        }

        void OnMove(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
            unit.movement = value.Get<Vector2>() * speed;
        }

        void Die() {
            GameManager.Instance.SetGameOver();
        }
    }
}