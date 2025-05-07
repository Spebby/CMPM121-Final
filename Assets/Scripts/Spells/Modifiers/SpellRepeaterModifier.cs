using System;
using CMPM.Core;
using CMPM.Structures;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellRepeaterModifier : SpellModifier {
        RPNString Delay;
        RPNString Counts;
        
        public SpellRepeaterModifier(RPNString repeats, RPNString delay,
                                     float damageMultiplier = 1f,
                                     float manaMultiplier = 1f,
                                     float speedMultiplier = 1f,
                                     float cooldownMultiplier = 1f,
                                     float lifetimeMultiplier = 1f) 
            : base(damageMultiplier, manaMultiplier, 
                   speedMultiplier, cooldownMultiplier,
                   lifetimeMultiplier) {
            Counts = repeats;
            Delay = delay;
        }

        // This is something to ask Markus about tomorrow.
        public virtual void ModifyCast(Spell spell, ref Action<Vector3, Vector3> original) {
            Hashtable<string, float> table = spell.GetRPNVariables();
            original = (where, target) => {
                // It's tempting to start a new coroutine, but because we're technically in a corooutine already... that's bad.
                
                // Ask markus about good ways to remedy this design flaw
            };
        }
        
        
 
    }
}