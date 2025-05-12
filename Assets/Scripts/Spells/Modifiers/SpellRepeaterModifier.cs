using System;
using System.Collections;
using CMPM.Core;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellRepeaterModifier : SpellModifier {
        protected readonly RPNString Delay;
        protected readonly RPNString Count;
        
        public SpellRepeaterModifier(RPNString repeats, RPNString delay,
                                     RPNString? damageModifier   = null,
                                     RPNString? manaModifier     = null,
                                     RPNString? speedModifier    = null,
                                     RPNString? cooldownModifier = null,
                                     RPNString? lifetimeModifier = null)
            : base(damageModifier, manaModifier, 
                   speedModifier, cooldownModifier,
                   lifetimeModifier) {
            Count = repeats;
            Delay = delay;
        }

        public override void ModifyCast(Spell spell, ref Action<ProjectileType, Vector3, Vector3> original) {
            SerializedDictionary<string, float> table = spell.GetRPNVariables();
            
			Action<ProjectileType, Vector3, Vector3> prev = original;
            original = (type, where, target) => {
                float delay = Delay.Evaluate(table);
                int count = (int)Count.Evaluate(table);
                CoroutineManager.Instance.Run(SpawnProjectileDelay(delay, count, prev, type, where, target));
            };
        }

        static IEnumerator SpawnProjectileDelay(float delay, int count, Action<ProjectileType, Vector3, Vector3> action, ProjectileType type, Vector3 where, Vector3 target) {
            for (int i = 0; i < count; i++) {
                action(type, where, target);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
