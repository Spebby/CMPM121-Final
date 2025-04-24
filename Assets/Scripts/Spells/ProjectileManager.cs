using System;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Projectiles;
using UnityEngine;


namespace CMPM.Spells {
    public class ProjectileManager : MonoBehaviour {
        public GameObject[] projectiles;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            GameManager.INSTANCE.ProjectileManager = this;
        }

        // Update is called once per frame
        void Update() { }

        public void CreateProjectile(
            int which, string trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable, Vector3> onHit) {
            GameObject newProjectile = Instantiate(projectiles[which], where + direction.normalized * 1.1f,
                                                   Quaternion.Euler(
                                                       0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            newProjectile.GetComponent<ProjectileController>().Movement =  MakeMovement(trajectory, speed);
            newProjectile.GetComponent<ProjectileController>().OnHit    += onHit;
        }

        public void CreateProjectile(
            int which, string trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable, Vector3> onHit,
            float lifetime) {
            GameObject newProjectile = Instantiate(projectiles[which], where + direction.normalized * 1.1f,
                                                   Quaternion.Euler(
                                                       0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            newProjectile.GetComponent<ProjectileController>().Movement =  MakeMovement(trajectory, speed);
            newProjectile.GetComponent<ProjectileController>().OnHit    += onHit;
            newProjectile.GetComponent<ProjectileController>().SetLifetime(lifetime);
        }

        public ProjectileMovement MakeMovement(string type, float speed) {
            return type switch {
                "straight"  => new StraightProjectileMovement(speed),
                "homing"    => new HomingProjectileMovement(speed),
                "spiraling" => new SpiralingProjectileMovement(speed),
                _           => null
            };
        }
    }
}