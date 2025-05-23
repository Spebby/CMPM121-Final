using System;
using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.Spells.Modifiers;
using CMPM.Utils;
using UnityEngine;


namespace CMPM.Spells {
    public class IceBolt : Spell
    {
        protected readonly float SlowFactor;
        protected readonly int TimeSlowed;

        public IceBolt(SpellCaster owner, string name, RPNString manaCost, RPNString damage,
                       Damage.Type damageDamageType, RPNString speed, RPNString cooldown, RPNString? lifetime,
                       float? slowFactor, int? timeSlowed,
                       uint icon, int[] modifiers = null) : base(owner, name, manaCost, damage, damageDamageType,
                                                                 speed, cooldown, lifetime, icon, modifiers)
        {
            SlowFactor = (float)slowFactor;
            TimeSlowed = (int)timeSlowed;                                                 
        }

        public virtual float GetSlowFactor()
        {
            return SlowFactor;
        }

        public virtual int GetTimeSlowed()
        {
            return TimeSlowed;
        }

        IEnumerator ApplySlow (Hittable other)
        {
            int normal_enemy_speed = other.Owner.GetComponent<EnemyController>().speed;
            other.Owner.GetComponent<EnemyController>().speed = Mathf.RoundToInt(other.Owner.GetComponent<EnemyController>().speed * GetSlowFactor());
            yield return new WaitForSeconds(GetTimeSlowed());
            try
            {
                other.Owner.GetComponent<EnemyController>().speed = normal_enemy_speed;
            }
            catch
            {/* o.owner died before slow is done oh well */}
        }

        protected override void OnHit(Hittable other, Vector3 impact)
        {
            if (other.team == Team) return;
            Action<Hittable, Vector3, Damage.Type> hitAction = (o, i, t) =>
            {
                o.Damage(new Damage(GetDamage(), t));

                if (o.team == Hittable.Team.MONSTERS)
                {
                    CoroutineManager.Instance.Run(ApplySlow(o));
                }
            };

            foreach (int hash in Modifiers ?? Array.Empty<int>())
            {
                ISpellModifier mod = SpellModifierRegistry.Get(hash);
                mod?.ModifyHit(this, ref hitAction);
            }

            hitAction(other, impact, DamageType);
        }
    }
}