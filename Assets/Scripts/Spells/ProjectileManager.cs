using System;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Projectiles;
using UnityEngine;


namespace CMPM.Spells {
    public enum ProjectileType {
        STRAIGHT,
        HOMING,
        SPIRALING
    }
    
    // This would be fun to object pool.
    public class ProjectileManager : MonoBehaviour {
        [SerializeField] GameObject[] projectiles;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            GameManager.Instance.ProjectileManager = this;
        }

        public void CreateProjectile(
            int which, ProjectileType trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable, Vector3> onHit) {
            GameObject newProjectile = Instantiate(projectiles[which], where + direction.normalized * 1.1f,
                                                   Quaternion.Euler(
                                                       0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            newProjectile.GetComponent<ProjectileController>().Movement =  MakeMovement(trajectory, speed);
            newProjectile.GetComponent<ProjectileController>().OnHit    += onHit;
        }

        public void CreateProjectile(
            int which, ProjectileType trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable, Vector3> onHit,
            float lifetime) {
            GameObject newProjectile = Instantiate(projectiles[which], where + direction.normalized * 1.1f,
                                                   Quaternion.Euler(
                                                       0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            newProjectile.GetComponent<ProjectileController>().Movement =  MakeMovement(trajectory, speed);
            newProjectile.GetComponent<ProjectileController>().OnHit    += onHit;
            newProjectile.GetComponent<ProjectileController>().SetLifetime(lifetime);
        }

        public static ProjectileMovement MakeMovement(ProjectileType type, float speed) {
            return type switch {
                ProjectileType.STRAIGHT  => new StraightProjectileMovement(speed),
                ProjectileType.HOMING    => new HomingProjectileMovement(speed),
                ProjectileType.SPIRALING => new SpiralingProjectileMovement(speed),
                _                        => null
            };
        }
    }
}