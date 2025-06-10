using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace CMPM.MapGenerator {
    [CreateAssetMenu(fileName = "RoomArchetype", menuName = "MapGenerator/RoomArchetype")]
    public class RoomArchetype : ScriptableObject {
        public Room[] rooms;
        public Door[] doors;
        public int Weight = 1;
        public int Width  = 1;
        public int Height = 1;
        [SerializeField] internal bool[] Occupancy = {true};
        
        // Used for caching in map generation. Assume these are filled with garbage data at all times.
        Door[] _doorBuf;
        Vector2Int[] _occBuf;

        public IReadOnlyList<Room> GetAllRooms() => rooms;
        
        #region Generator Helpers
        public ReadOnlySpan<Vector2Int> GetOccupancy(in Vector2Int offset) {
            if (_occBuf == null || _occBuf.Length < Occupancy.Length)
                _occBuf = new Vector2Int[Occupancy.Length];

            // No bump b/c occupancy is in grid cords
            for (int i = 0; i < Occupancy.Length; i++) {
                if (!Occupancy[i]) continue;
                _occBuf[i] = new Vector2Int(
                    (i % Width) + offset.x,
                    (i / Width) + offset.y
                );
            }
            
            return _occBuf.AsSpan(0, _occBuf.Length);
        }
        
        public int GetDoorCount() => doors.Length;
        
        public ReadOnlySpan<Door> GetDoors(Vector2Int offset) {
            if (_doorBuf == null || _doorBuf.Length != doors.Length)
                _doorBuf = new Door[doors.Length];

            // Offset and return
            for (int i = 0; i < _doorBuf.Length; i++) {
                Door d = doors[i];
                _doorBuf[i] = new Door(d.GridCoordinates + offset, d.GetDirection());
            }

            return _doorBuf.AsSpan(0, _doorBuf.Length);
        }
        
        public bool HasDoorOnSide(Door.Direction direction) {
            for (int i = 0; i < doors.Length; i++) {
                if (doors[i].GetDirection() == direction) return true;
            }

            return false;
        }
        
        public ReadOnlySpan<Door> GetHallwaySideDoors(in Vector2Int offset) {
            Door[] buf  = new Door[doors.Length];

            int count = 0;
            for (int i = 0; i < doors.Length; i++) {
                if (doors[i].GetDirection() == Door.Direction.NORTH) {
                    buf[count++] = new Door(doors[i].GridCoordinates + offset, Door.Direction.NORTH);
                } else if (doors[i].GetDirection() == Door.Direction.EAST) {
                    buf[count++] = new Door(doors[i].GridCoordinates + offset, Door.Direction.EAST);
                }
            }

            return buf.AsSpan(0, count); 
        }
        #endregion
        
        public Room Get(int i) => rooms[i];

        public Room GetRandomRoom(in System.Random rng) {
            return GetRandomRoom(rooms, rng);
        }

        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        static Room GetRandomRoom(in Room[] collection, in System.Random rng) {
            int totalWeight = collection.Sum(room => room.Weight);
            if (totalWeight == 0) throw new InvalidOperationException("All room weights are zero.");

            int pick  = rng.Next(totalWeight);
            int accum = 0;

            for (int i = 0; i < collection.Length; i++) {
                Room room = collection[i];
                accum += room.Weight;
                if (pick < accum) return room;
            }

            return collection[^1]; // fallback (shouldn’t happen)
        }
        
        internal void ResizeGrid(int newWidth, int newHeight) {
            if (newWidth < 1 || newHeight < 1) throw new ArgumentOutOfRangeException();
            bool[] newOccupancy = new bool[newWidth * newHeight];
            for (int y = 0; y < Mathf.Min(Height, newHeight); y++) {
                for (int x = 0; x < Mathf.Min(Width, newWidth); x++) {
                    int oldIndex = y * Width + x;
                    int newIndex = y * newWidth + x;
                    newOccupancy[newIndex] = Occupancy != null && oldIndex < Occupancy.Length && Occupancy[oldIndex];
                }
            }

            Width        = newWidth;
            Height       = newHeight;
            Occupancy    = newOccupancy;
        }
        
#if UNITY_EDITOR
        void OnValidate() {
            for (int i = 0; i < doors.Length; i++) {
                ref Door d = ref doors[i];
                d.WorldCoordinates = Door.GetWorldCoordinates(d.GridCoordinates, d.GetDirection());
            }
        }
#endif
    }
}