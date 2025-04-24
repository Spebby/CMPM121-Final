using System;
using UnityEngine;


namespace CMPM.DamageSystem {
    public class Hittable {
        public enum Team {
            PLAYER,
            MONSTERS
        }

        // ReSharper disable once InconsistentNaming
        public Team team;

        public int Hp;
        public int MaxHp;

        public readonly GameObject Owner;

        public void Damage(Damage damage) {
            EventBus.Instance.DoDamage(Owner.transform.position, damage, this);
            Hp -= damage.Amount;
            if (Hp > 0) return;
            Hp = 0;
            OnDeath?.Invoke();
        }

        public event Action OnDeath;

        public Hittable(int hp, Team team, GameObject owner) {
            Hp        = hp;
            MaxHp     = hp;
            this.team = team;
            Owner     = owner;
        }

        public void SetMaxHp(int maxHp) {
            float ratio = Hp * 1.0f / MaxHp;
            MaxHp = maxHp;
            Hp    = Mathf.RoundToInt(ratio * maxHp);
        }
    }
}