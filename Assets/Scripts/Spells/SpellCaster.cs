using System.Collections;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public class SpellCaster {
        public int Mana;
        public int MaxMana;
        public int ManaReg;
        public int SpellPower;
        public readonly Hittable.Team Team;
        public readonly Spell Spell;

        public IEnumerator ManaRegeneration() {
            while (true) {
                Mana += ManaReg;
                Mana =  Mathf.Min(Mana, MaxMana);
                yield return new WaitForSeconds(1);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public SpellCaster(int mana, int manaReg, int spellPower, Hittable.Team team) {
            Mana       = mana;
            MaxMana    = mana;
            ManaReg    = manaReg;
            SpellPower = spellPower;
            Team       = team;
            Spell      = new SpellBuilder().Build(this);
        }

        // 
        public IEnumerator Cast(Vector3 where, Vector3 target) {
            if (Mana < Spell.GetManaCost() || !Spell.IsReady()) yield break;
            Mana -= Spell.GetManaCost();
            yield return Spell.Cast(where, target, Team);
        }
    }
}