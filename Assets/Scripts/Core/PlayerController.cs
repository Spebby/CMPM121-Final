using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CMPM.DamageSystem;
using CMPM.Relics;
using CMPM.Spells;
using CMPM.UI;
using CMPM.Utils;
using CMPM.Utils.Structures;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Unit = CMPM.Movement.Unit;


namespace CMPM.Core {
    public class PlayerController : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        public Hittable HP;
        [SerializeField] SpriteRenderer spriteRenderer;
        SpellCaster _caster;

        public readonly struct PlayerClass {
            public readonly Type @Class;
            public readonly string Description;
            public readonly uint Sprite;
            public readonly RPNString Health;
            public readonly RPNString MaxMana;
            public readonly RPNString ManaRegen;
            public readonly RPNString Spellpower;
            public readonly RPNString Speed;

            public PlayerClass(Type type, string description, uint sprite, RPNString health, RPNString maxMana,
                               RPNString manaRegen, RPNString spellpower, RPNString speed) {
                @Class      = type;
                Description = description;
                Sprite      = sprite;
                Health      = health;
                MaxMana     = maxMana;
                ManaRegen   = manaRegen;
                Spellpower  = spellpower;
                Speed       = speed;
            }

            [JsonConverter(typeof(PlayerClassTypeParser))]
            public enum Type {
                Mage,
                Warlock,
                Battlemage
            }
        }

        PlayerClass Class { get; set; }
        public int Speed { get; private set; }
        [SerializeField] Unit unit;

        #region UI
        // ReSharper disable once StringLiteralTypo
        [SerializeField] [FormerlySerializedAs("healthui")] HealthBar healthUI;

        // ReSharper disable once StringLiteralTypo
        [SerializeField] [FormerlySerializedAs("manaui")] ManaBar manaUI;

        // ReSharper disable once StringLiteralTypo
        [SerializeField] SpellUIContainer spellUI;
        #endregion

        #region APIs
        public bool IsMoving() => unit.movement.magnitude > Mathf.Epsilon;
        public void ModifySpeed(int c) => Speed = Mathf.Max(Speed + c, 1);
        public static implicit operator SpellCaster(PlayerController player) => player._caster;
        public void UpdateClass(in PlayerClass c) {
            Class                = c;
            spriteRenderer.sprite = GameManager.Instance.PlayerSpriteManager.Get(Class.Sprite);
            SetPlayerScale(GameManager.Instance.CurrentWave);
        }
        #endregion

        void Start() {
            unit                                  = GetComponent<Unit>();
            GameManager.Instance.Player           = gameObject;
            GameManager.Instance.PlayerController = this;
            HP                                    = new Hittable(100, Hittable.Team.PLAYER, gameObject);
            RelicOwnership                        = new BitArray(RelicRegistry.Count);
        }

        public void StartLevel() {
            Spell activeSpell = _spells[_spellIndex];
            _caster = new SpellCaster(125, 8, 1, Hittable.Team.PLAYER, activeSpell);
            StartCoroutine(_caster.ManaRegeneration());
            _spells[_spellIndex] = _caster.Spell;

            int wave = GameManager.Instance.CurrentWave;
            SetPlayerScale(wave);

            HP.HealToMax();
            HP.OnDeath += Die;
            HP.team    =  Hittable.Team.PLAYER;

            // tell UI elements what to show
            spellUI.AddSpell(_caster.Spell, _spellIndex);
            healthUI.SetHealth(HP);
            manaUI.SetSpellCaster(_caster);
        }

        void SetPlayerScale(int wave) {
            if (_caster == null) return;
            SerializedDictionary<string, int> table = new() { { "wave", wave } };
            HP.UpdateHPCap(Class.Health.Evaluate(table));
            _caster.MaxMana   = Class.MaxMana.Evaluate(table);
            _caster.ManaRegen = Class.ManaRegen.Evaluate(table);
            _caster.AddSpellpower(Class.Spellpower.Evaluate(table));
            Speed = Class.Speed.Evaluate(table);
        }

        #region Spells
        int _spellIndex;

        readonly List<int> _baseSpellModifiers = new();
        readonly Spell[] _spells = new Spell[4];
        /* To note about readonly: you can think of it as preventing a change to the value of a variable
         * for value types, this is straight forwards. For reference types, like an array, this means
         * the pointer cannot be changed--so you cannot reallocate the array. But you *can* still change
         * the value of what the reference is pointing to. */

        public void AddSpell(in Spell spell, int replaceIndex = 0) {
            if (replaceIndex < 0) throw new ArgumentOutOfRangeException(nameof(replaceIndex));
            for (int i = 0; i < _spells.Length; ++i) {
                if (_spells[i] != null) continue;
                _spells[i] = spell;
                spellUI.AddSpell(_spells[i], i);
                return;
            }

            spell.AddModifiers(_baseSpellModifiers.ToArray());
            _spells[replaceIndex] = spell;
            spellUI.AddSpell(_spells[replaceIndex], replaceIndex);
        }

        public void AddBaseSpellModifiers(int[] modifiers) {
            _baseSpellModifiers.AddRange(modifiers);
            foreach (Spell spell in _spells) {
                spell?.AddModifiers(modifiers);
            }
        }

        public void RemoveBaseSpellModifiers(int[] modifiers) {
            foreach (int modifier in modifiers) {
                int index = _baseSpellModifiers.IndexOf(modifier);
                if (index == -1) continue;
                _baseSpellModifiers.RemoveAt(index);
            }

            foreach (Spell spell in _spells) {
                spell.RemoveModifiers(modifiers);
            } 
        }
        
        public void ClearSpells() {
            for (int i = 0; i < _spells.Length; ++i) {
                _spells[i] = null;
                spellUI.spellUIs[i].SetSpell(null);
            }
        }

        public void SwitchSpell(int i) {
            _caster.Spell = _spells[i];
            _spellIndex        = i;
            spellUI.SetSpellAsActive(_spellIndex);
        }

        public void DropSpell(int i) {
            _spells[i] = null;
            spellUI.AddSpell(null, i);
            NextSpell(i);
            spellUI.SetSpellAsActive(_spellIndex);
        }

        void NextSpell(int start = 0) {
            for (int offset = 0; offset < _spells.Length; offset++) {
                int i = (start + offset) % _spells.Length;
                if (_spells[i] == null) continue;
                _caster.Spell = _spells[i];
                _spellIndex        = i;
                spellUI.SetSpellAsActive(i);
                return;
            }

            throw new Exception("Spell Caster has no spells!");
        }

        public bool HasSpellRoom() {
            return _spells.Any(t => t == null);
        }

        public Spell[] GetSpells() {
            return _spells;
        }
        #endregion

        #region Relics
        public readonly List<Relic> Relics = new();
        public BitArray RelicOwnership { get; private set; }
        
        public void AddRelic(Relic relic) {
            Relics.Add(relic);
            RelicOwnership.Set(RelicRegistry.GetIndexFromRelic(relic), true);
            EventBus.Instance.DoRelicPickup(relic);
        } 
        #endregion
        
        #region Input Callbacks
        // ReSharper disable once UnusedMember.Local
        void OnAttack(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
            Vector2 mouseScreen = Mouse.current.position.value;
            Vector3 mouseWorld  = Camera.main!.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0;
            StartCoroutine(_caster.Cast(transform.position, mouseWorld));
        }
        
        // ReSharper disable once UnusedMember.Local
        void OnChangeSpell(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;

            NextSpell(++_spellIndex);
        }

        // ReSharper disable once UnusedMember.Local
        void OnMove(InputValue value) {
            if (GameManager.Instance.State == GameManager.GameState.PREGAME
             || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
            unit.movement = value.Get<Vector2>() * Speed;
            EventBus.Instance.DoPlayerMove(unit.movement.magnitude);
            EventBus.Instance.DoPlayerStandstill();
        }
        #endregion

        // ReSharper disable once MemberCanBeMadeStatic.Local
        void Die() {
            GameManager.Instance.SetGameOver();
        }
    }
}