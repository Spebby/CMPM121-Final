using System;
using CMPM.Core;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.DamageSystem {
    [Serializable]
    public class Hittable {
        public enum Team {
            PLAYER,
            MONSTERS
        }

        // ReSharper disable once InconsistentNaming
        public Team team;

        [FormerlySerializedAs("Hp")] public int HP;
        [FormerlySerializedAs("MaxHp")] public int MaxHP;
        [FormerlySerializedAs("MinHp")] public int MinHP;

        public readonly GameObject Owner;

        public void Damage(Damage damage) {
            EventBus.Instance.DoDamage(Owner.transform.position, damage, this);
            HP -= damage.Amount;
            if (HP < MinHP) MinHP = HP;
            if (HP > 0) return;
            HP = 0;
            OnDeath?.Invoke();
        }

        public void Heal(int amount) {
            // no resurrection
            if (HP <= 0) return;

            // no overhealing
            if (MaxHP - HP < amount) amount = MaxHP - HP;
            if (amount == 0) return;
            EventBus.Instance.DoHeal(Owner.transform.position, amount, this);
            HP += amount;
        }

        public event Action OnDeath;

        public Hittable(int hp, Team team, GameObject owner) {
            HP        = hp;
            MaxHP     = hp;
            MinHP     = hp;
            this.team = team;
            Owner     = owner;
        }

        public void UpdateHPCap(int max) {
            float ratio = HP * 1.0f / MaxHP;
            MaxHP = max;
            MinHP = max;
            HP    = Mathf.RoundToInt(ratio * max);
        }

        public void HealToMax() {
            HP = MaxHP;
        }
    }
}