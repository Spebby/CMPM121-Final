using System.Collections;
using CMPM.Core;
using CMPM.DamageSystem;
using UnityEngine;


namespace CMPM.Spells {
    public class Spell {
        public float LastCast;
        public SpellCaster Owner;
        public Hittable.Team Team;

        public Spell(SpellCaster owner, string name, int manaCost, int damage, float cooldown) {
            Owner = owner;
            Name = name;
            ManaCost = manaCost;
            Damage = damage;
            Cooldown = cooldown;
            Icon = icon;
        }

        public string GetName() {
            return Name;
        }

        public int GetManaCost() {
            return ManaCost;
        }

        public int GetDamage() {
            return Damage;
        }

        public float GetCooldown() {
            return Cooldown;
        }

        public virtual int GetIcon() {
            return Icon;
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