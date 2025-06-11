using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;
using static CMPM.Core.GameManager.GameState;


namespace CMPM.Spells {
    public class SpellCaster {
        public int Mana { get; private set; }
        public int MaxMana { get; set; }
        public int ManaRegen { get; set; }
        public int SpellPower { get; private set; }
        public readonly Hittable.Team Team;
        
        public Spell Spell;
        const string DEFAULT_SPELL = "Arcane Bolt";

        // ReSharper disable once UnassignedField.Global
        public Action OnCast;
        
        public IEnumerator ManaRegeneration() {
            while (true) {
                Mana += ManaRegen;
                Mana =  Mathf.Min(Mana, MaxMana);
                yield return new WaitForSeconds(1);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public SpellCaster(int mana, int manaRegen, int spellPower, Hittable.Team team, Spell spell) {
            Mana       = mana;
            MaxMana    = mana;
            ManaRegen  = manaRegen;
            SpellPower = spellPower;
            Team       = team;
            Spell      = spell ?? SpellBuilder.BuildSpell(DEFAULT_SPELL, this, 0);
        }

        public IEnumerator Cast(Vector3 where, Vector3 target) {
            if (Mana < Spell.GetManaCost() || !Spell.IsReady() ||
                GameManager.Instance.State.HasFlag(INGAME | INCOMBAT)) yield break;
            Mana -= Spell.GetManaCost();
            OnCast?.Invoke();
            yield return Spell.Cast(where, target, Team);
        }

        public void AddMana(int c) {
            Mana = Mathf.Clamp(Mana + c, 0, MaxMana);
        }

        public void AddMaxMana(int c) {
            MaxMana = Mathf.Max(MaxMana + c, 0);
        }

        // Arguably we may want "negative" mana regen as a feature but for the moment I don't see any value in that.
        public void AddManaRegen(int c) {
            ManaRegen = Mathf.Max(ManaRegen + c, 0); 
        }
        
        public void AddSpellpower(int c) {
            SpellPower = Mathf.Max(SpellPower + c, 1);
        }
    }
}