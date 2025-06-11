using System;
using System.Collections.Generic;
using CMPM.Core;
using CMPM.Level;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


namespace CMPM.MapGenerator {
    public sealed class Room : MonoBehaviour {
        internal const int GRID_SIZE = 22;
        [SerializeField] Tilemap tiles;
        [FormerlySerializedAs("weight"), SerializeField] internal int Weight;

        public RoomSpawn spawns;
        [HideInInspector] public SpawnPoint[] spawnpoints;
        GameObject[] _lockedDoors;
        
        public bool Cleared { get; private set; }
        
        void Awake() {
            spawnpoints  = GetComponentsInChildren<SpawnPoint>();
            _lockedDoors = transform.GetChildrenWithTag("door").ToArray();
            if (spawns is null) Cleared = true;
            foreach (GameObject door in _lockedDoors ?? Array.Empty<GameObject>()) {
                door.SetActive(false);
            } 
        }
        
        
        public Vector2Int GetPivotCoordinates() => new((int)transform.position.x / GRID_SIZE, (int)transform.position.y / GRID_SIZE);
        
        internal GameObject Place(in Vector2Int where) {
            Room newRoom = Instantiate(this, new Vector3(where.x * GRID_SIZE, where.y * GRID_SIZE), Quaternion.identity);
            return newRoom.gameObject;
        }

        public void UnlockDoors() {
            Cleared = true;
            foreach (GameObject door in _lockedDoors ?? Array.Empty<GameObject>()) {
                door.SetActive(false);
            }
        }
        
        public void LockDoors() {
            Cleared = false;
            foreach (GameObject door in _lockedDoors ?? Array.Empty<GameObject>()) {
                door.SetActive(true);
            }
        }
        
        void OnTriggerEnter2D(Collider2D col) {
            if (GameManager.Instance.State != GameManager.GameState.INGAME || Cleared) return;
            LockDoors();
            EnemySpawner.Instance.SpawnEnemies(this);
        }

#if UNITY_EDITOR
        void OnDrawGizmos() {
            if (spawnpoints != null) {
                foreach (SpawnPoint spawn in spawnpoints) {
                    Gizmos.color = spawn.Kind switch {
                        SpawnPoint.SpawnName.RED    => Color.red,
                        SpawnPoint.SpawnName.GREEN  => Color.green,
                        SpawnPoint.SpawnName.BONE   => Color.gray,
                        SpawnPoint.SpawnName.RANDOM => Color.magenta,
                        _                           => throw new ArgumentOutOfRangeException()
                    };
                    Gizmos.DrawWireSphere(spawn.transform.position, 0.25f);
                }
            }

            Gizmos.color = Color.red;
            /*
            if (_occupancy == null) return;
            Vector3    transformPosition = gameObject.transform.position;
            Vector2Int offset            = new((int)transformPosition.x, (int)transformPosition.y);
            ReadOnlySpan<Door> doors     = GetDoors(offset);

            Gizmos.color = Color.red;
            for (int i =  0; i <  doors.Length; ++i) {
                Vector3 v = doors[i].GetLocalCoordinates();
                Gizmos.DrawSphere(new Vector3(v.x + 0.5f, v.y + 0.5f), 0.5f);
            }

            // Will have to update this when we do non-rectangular rooms.
            {
                Gizmos.color   = Color.magenta;
                Vector3    dim = new(Width * GRID_SIZE - 1, Height * GRID_SIZE - 1);
                Gizmos.DrawWireCube((dim * 0.5f) + transformPosition, new Vector3(dim.x, dim.y, 0));
            }
            */
        }
        #endif
    }

    public static class TransformExtensions {
        public static List<GameObject> GetChildrenWithTag(this Transform parent, string tag) {
            List<GameObject> taggedChildren = new();

            foreach (Transform child in parent) {
                if (child.CompareTag(tag))
                    taggedChildren.Add(child.gameObject);

                taggedChildren.AddRange(child.GetChildrenWithTag(tag));
            }

            return taggedChildren;
        }
    }
}