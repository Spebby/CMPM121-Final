using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public class SpellCaster {
        public int Mana { get; private set; }
        public int MaxMana { get; set; }
        public int ManaReg { get; set; }
        public int SpellPower { get; private set; }
        public readonly Hittable.Team Team;
        
        public Spell Spell;
        const string DEFAULT_SPELL = "Arcane Bolt";

        // ReSharper disable once UnassignedField.Global
        public Action OnCast;
        
        public IEnumerator ManaRegeneration() {
            while (true) {
                Mana += ManaReg;
                Mana =  Mathf.Min(Mana, MaxMana);
                yield return new WaitForSeconds(1);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public SpellCaster(int mana, int manaReg, int spellPower, Hittable.Team team, Spell spell) {
            Mana       = mana;
            MaxMana    = mana;
            ManaReg    = manaReg;
            SpellPower = spellPower;
            Team       = team;
            Spell      = spell ?? SpellBuilder.BuildSpell(DEFAULT_SPELL, this, 0);
        }

        public IEnumerator Cast(Vector3 where, Vector3 target) {
            if (Mana < Spell.GetManaCost() || !Spell.IsReady() ||
                GameManager.Instance.State != GameManager.GameState.INWAVE) yield break;
            Mana -= Spell.GetManaCost();
            OnCast?.Invoke();
            yield return Spell.Cast(where, target, Team);
        }

        public void GainMana(int c) {
            Mana = Mathf.Clamp(Mana + c, 0, MaxMana);
        }

        public void GainSpellpower(int c) {
            SpellPower = Mathf.Max(SpellPower + c, 1);
        }
    }
}