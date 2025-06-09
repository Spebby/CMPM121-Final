using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


namespace CMPM146.MapGenerator {
    public sealed class Room : MonoBehaviour {
        public const int GRID_SIZE = 12;
        public Tilemap tiles;
        [FormerlySerializedAs("weight"), SerializeField] internal int Weight;

        public Vector2Int GetPivotCoordinates() => new((int)transform.position.x / GRID_SIZE, (int)transform.position.y / GRID_SIZE);
        
        public GameObject Place(in Vector2Int where) {
            Room newRoom = Instantiate(this, new Vector3(where.x * GRID_SIZE, where.y * GRID_SIZE), Quaternion.identity);
            return newRoom.gameObject;
        }

        
        /*
        int _width;
        int _height;
        bool[] _occupancy;
        
        public Vector2Int GetGridSize() => new(_width, _height);
        #if UNITY_EDITOR
        void OnDrawGizmos() {
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
        }
        #endif
        */
    }
}