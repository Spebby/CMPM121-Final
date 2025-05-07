using System;
using System.Collections;
using CMPM.Structures;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellSplitModifier : SpellModifier {
        readonly RPNString _count;
        readonly RPNString _spread;

        public SpellSplitModifier(RPNString count, RPNString spread,
                                  float damageMultiplier = 1f,
                                  float manaMultiplier = 1f,
                                  float speedMultiplier = 1f,
                                  float cooldownMultiplier = 1f,
                                  float lifetimeMultiplier = 1f) 
            : base(damageMultiplier, manaMultiplier, 
                   speedMultiplier, cooldownMultiplier,
                   lifetimeMultiplier) {
            _count  = count;
            _spread = spread;
        }

        public override void ModifyCast(Spell spell, ref Action<Vector3, Vector3> original) {
            Hashtable<string, float> table  = spell.GetRPNVariables();
            uint                     count  = (uint)_count.Evaluate(table);
            float                    spread = _spread.Evaluate(table);

            if (count <= 1) return;
            
            Action<Vector3, Vector3> prev = original;
            original = (where, target) => {
                Vector3 delta = target - where;
                float distance = delta.magnitude;
                Vector3 dir = delta.normalized;

                for (int i = 0; i < count; i++) {
                    float t = i / (float)count;
                    float angle= ((t - 0.5f) * spread) * Mathf.Rad2Deg;
                    Vector3 shotDir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
                    Vector3 shotTarget = where + shotDir * distance;
                    prev (where, shotTarget);
                }
            };
        }
    }
}