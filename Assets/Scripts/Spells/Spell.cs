using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public class Spell {
        public float LastCast;
        public SpellCaster Owner;
        public Hittable.Team Team;

        public Spell(SpellCaster owner) {
            Owner = owner;
        }

        public string GetName() {
            return "Bolt";
        }

        public int GetManaCost() {
            return 10;
        }

        public int GetDamage() {
            return 100;
        }

        public float GetCooldown() {
            return 0.75f;
        }

        public virtual int GetIcon() {
            return 0;
        }

        public bool IsReady() {
            return LastCast + GetCooldown() < Time.time;
        }

        public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
            Team = team;
            GameManager.INSTANCE.ProjectileManager.CreateProjectile(0, "straight", where, target - where, 15f, OnHit);
            yield return new WaitForEndOfFrame();
        }

        void OnHit(Hittable other, Vector3 impact) {
            if (other.team == Team) return;
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }
    }
}