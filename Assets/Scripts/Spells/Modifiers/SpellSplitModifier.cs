using System;
using CMPM.Utils;
using CMPM.Utils.Structures;
using UnityEngine;


namespace CMPM.Spells.Modifiers {
    public class SpellSplitModifier : SpellModifier {
        readonly RPNString _count;
        readonly RPNString _spread;

        public SpellSplitModifier(RPNString count, RPNString spread,
                                  RPNString? damageModifier = null,
                                  RPNString? manaModifier = null,
                                  RPNString? speedModifier = null,
                                  RPNString? cooldownModifier = null,
                                  RPNString? lifetimeModifier = null)
            : base(damageModifier, manaModifier,
                   speedModifier, cooldownModifier,
                   lifetimeModifier) {
            _count  = count;
            _spread = spread;
        }

        public override void ModifyCast(Spell spell, ref Action<ProjectileType, Vector3, Vector3> original) {
            SerializedDictionary<string, float> table  = spell.GetRPNVariables();
            uint                                count  = (uint)_count.Evaluate(table);
            float                               spread = _spread.Evaluate(table);

            if (count <= 1) return;

            Action<ProjectileType, Vector3, Vector3> prev = original;
            original = (type, where, target) => {
                Vector3 delta    = target - where;
                float   distance = delta.magnitude;
                Vector3 dir      = delta.normalized;

                Vector3 arcAxis                      = Vector3.Cross(dir, Vector3.up);
                if (arcAxis == Vector3.zero) arcAxis = Vector3.right; // Fallback if shooting straight up/down

                for (int i = 0; i < count; i++) {
                    float   t          = i / (float)count;
                    float   angle      = (t - 0.5f) * spread; // Still in degrees
                    Vector3 shotDir    = Quaternion.AngleAxis(angle, arcAxis) * dir;
                    Vector3 shotTarget = where + shotDir * distance;
                    prev(type, where, shotTarget);
                }
            };
        }
    }
}