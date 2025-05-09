using System;
using System.Linq;
using CMPM.DamageSystem;
using CMPM.Spells;
using CMPM.UI;
using CMPM.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Unit = CMPM.Movement.Unit;


namespace CMPM.Core {
    public class PlayerController : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        public Hittable HP;
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

        public static implicit operator SpellCaster(PlayerController player) => player._spellCaster;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            unit                        = GetComponent<Unit>();
            GameManager.Instance.Player = gameObject;
            HP = new Hittable(100,  Hittable.Team.PLAYER, this.gameObject);
        }

        public void StartLevel() {
            Spell activeSpell = _spells[_spellIndex];
            _spellCaster = new SpellCaster(125, 8, 1, Hittable.Team.PLAYER, activeSpell);
            StartCoroutine(_spellCaster.ManaRegeneration());

            int wave = GameManager.Instance.currentWave;
            SetPlayerScale(wave);
            
            HP.ResetHealth();
            HP.OnDeath += Die;
            HP.team    =  Hittable.Team.PLAYER;

            // tell UI elements what to show
            RefreshUI();
        }

        void SetPlayerScale(int wave) {
            HP.SetMaxHp(RPN.Evaluate("95 wave 5 * +", new SerializedDictionary<string, int> { { "wave", wave } }));
            _spellCaster.MaxMana    = RPN.Evaluate("90 wave 10 * +", new SerializedDictionary<string, int> { { "wave", wave } });
            _spellCaster.ManaReg    = RPN.Evaluate("10 wave +", new SerializedDictionary<string, int> { { "wave", wave } });
            _spellCaster.SpellPower = RPN.Evaluate("wave 10 *", new SerializedDictionary<string, int> { { "wave", wave } });
            speed                   = 5;
        }
        
        #region Spells
        int _spellIndex = 0;
        readonly Spell[] _spells = new Spell[4];
        /* To note about readonly: you can think of it as preventing a change to the value of a variable
         * for value types, this is straight forwards. For reference types, like an array, this means
         * the pointer cannot be changed--so you cannot reallocate the array. But you *can* still change
         * the value of what the reference is pointing to. */
        
        void OnChangeSpell(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
            
            NextSpell(_spellIndex);
            RefreshUI();
        }
        
        public void AddNewSpell(Spell spell, int replaceIndex = 0) {
            if (replaceIndex < 0) throw new ArgumentOutOfRangeException(nameof(replaceIndex));
            for (int i = 0; i < _spells.Length; ++i) {
                if (_spells[i] != null) continue;
                _spells[i] = spell;
                return;
            }
            
            _spells[replaceIndex] = spell;
        }

        public void SwitchSpell(int i) {
            _spellCaster.Spell = _spells[i];
            _spellIndex = i;
        }

        public void DropSpell(int i) {
            _spells[i] = null;
            NextSpell(i);
        }

        void NextSpell(int start = 0) {
            for (int offset = 0; offset < _spells.Length; ++offset) {
                int i = (start + offset) %  _spells.Length;
                if (_spells[i] == null) continue;
                _spellCaster.Spell = _spells[i];
                _spellIndex = i;
                return;
            }

            throw new Exception("Spell Caster has no spells!");
        }

        public bool HasSpellRoom() => _spells.Any(t => t == null);
        #endregion
        
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

        // ReSharper disable once MemberCanBeMadeStatic.Local
        void Die() {
            GameManager.Instance.SetGameOver();
        }
        
        void RefreshUI() {
            healthUI.SetHealth(HP);
            manaUI.SetSpellCaster(_spellCaster);
            spellUI.SetSpell(_spellCaster.Spell);
        }
    }
}