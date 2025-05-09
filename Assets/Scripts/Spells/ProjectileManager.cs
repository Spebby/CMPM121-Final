using System;
using CMPM.Core;
using CMPM.DamageSystem;
using CMPM.Projectiles;
using UnityEngine;


namespace CMPM.Spells {
    public enum ProjectileType {
        STRAIGHT,
        HOMING,
        SPIRALING,
        SINE
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

        // Always pass 0 for which, for now, may change later
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

        public static ProjectileType StringToProjectileType(string type) {
            return type.ToLower() switch {
                "homing"    => ProjectileType.HOMING,
                "straight"  => ProjectileType.STRAIGHT,
                "spiraling" => ProjectileType.SPIRALING,
                "sine"      => ProjectileType.SINE,
                _           => throw new ArgumentException($"{type} is not a recognized type")
            };
        }
        
        public static ProjectileMovement MakeMovement(ProjectileType type, float speed) {
            return type switch {
                ProjectileType.STRAIGHT  => new StraightProjectileMovement(speed),
                ProjectileType.HOMING    => new HomingProjectileMovement(speed),
                ProjectileType.SPIRALING => new SpiralingProjectileMovement(speed),
                ProjectileType.SINE      => new SineProjectileMovement(speed),
                _                        => null
            };
        }
    }
}