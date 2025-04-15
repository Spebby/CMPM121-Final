using System.Collections;
using UnityEngine;


public class Spell {
    public float LastCast;
    public SpellCaster Owner;
    public Hittable.Team Team;

    public Spell(SpellCaster owner) {
        Owner = owner;
    }

    public string GetName() => "Bolt";

    public int GetManaCost() => 10;

    public int GetDamage() => 100;

    public float GetCooldown() => 0.75f;

    public virtual int GetIcon() => 0;

    public bool IsReady() => LastCast + this.GetCooldown() < Time.time;

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team) {
        Team = team;
        GameManager.Instance.ProjectileManager.CreateProjectile(0, "straight", where, target - where, 15f, this.OnHit);
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact) {
        if (other.team == Team) return;
        other.Damage(new Damage(this.GetDamage(), Damage.Type.ARCANE));
    }
}