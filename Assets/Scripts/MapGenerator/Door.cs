using System;
using UnityEngine;
using UnityEngine.Serialization;


// My original version of this class used bit packing :D
namespace CMPM.MapGenerator {
    [Serializable]
    public struct Door : IEquatable<Door> {
        public enum Direction {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }
        
        public static Direction[] Cardinals = {
            Direction.NORTH,
            Direction.SOUTH,
            Direction.EAST,
            Direction.WEST
        };

        public static Vector2Int DirToVec(Direction direction) {
            return direction switch {
                Direction.NORTH => Vector2Int.up,
                Direction.SOUTH => Vector2Int.down,
                Direction.EAST  => Vector2Int.right,
                Direction.WEST  => Vector2Int.left,
                _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
        
        public static Direction GetMatchingDirection(Direction direction) {
            return direction switch {
                Direction.NORTH => Direction.SOUTH,
                Direction.SOUTH => Direction.NORTH,
                Direction.EAST  => Direction.WEST,
                Direction.WEST  => Direction.EAST,
                _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        [FormerlySerializedAs("Coordinates"),SerializeField] internal Vector2Int GridCoordinates;
        [SerializeField] Direction direction;
        [HideInInspector, SerializeField] internal Vector2Int WorldCoordinates;
        
        public Door(Vector2Int gridCoordinates, Direction direction) {
            GridCoordinates = gridCoordinates;
            this.direction  = direction;
            WorldCoordinates = GetWorldCoordinates(gridCoordinates, direction);
        }

        public readonly Direction GetDirection() => direction;

        public Vector2Int GetGridCoordinates() => new(GridCoordinates.x, GridCoordinates.y);

        internal static Vector2Int GetWorldCoordinates(Vector2Int gridCoordinates, Direction direction) {
            int tX = gridCoordinates.x * Room.GRID_SIZE;
            int tY = gridCoordinates.y * Room.GRID_SIZE;
            return direction switch {
                Direction.NORTH => new Vector2Int(tX + 5, tY + 10),
                Direction.SOUTH => new Vector2Int(tX + 5, tY),
                Direction.EAST  => new Vector2Int(tX + 10, tY + 5),
                Direction.WEST  => new Vector2Int(tX, tY + 5),
                _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public readonly Vector3 GetLocalCoordinates() => new(WorldCoordinates.x, WorldCoordinates.y, 0);

        public Door GetMatching() {
            int x = GridCoordinates.x, y = GridCoordinates.y;
            // The +- 2 is b/c we have the padding of 1 unit for each room. Doors are guaranteed to touch that boundary
            // so we need to bump it over to the "correct" spot on the other side of the boundary.
            return direction switch {
                Direction.EAST  => new Door(new Vector2Int(x + 1, y), Direction.WEST),
                Direction.WEST  => new Door(new Vector2Int(x - 1, y), Direction.EAST),
                Direction.NORTH => new Door(new Vector2Int(x, y + 1), Direction.SOUTH),
                Direction.SOUTH => new Door(new Vector2Int(x, y - 1), Direction.NORTH),
                _               => throw new Exception("Unknown direction!")
            };
        }

        public bool IsMatching(Door other) {
            Door match = GetMatching();
            return match.GridCoordinates == other.GridCoordinates && match.direction == other.direction;
        }

        public Direction GetMatchingDirection() => GetMatchingDirection(direction);

        public override string ToString() => GetGridCoordinates() + " " + direction;

        public bool IsVertical() {
            return direction is Direction.NORTH or Direction.SOUTH;
        }

        public bool IsHorizontal() {
            return direction is Direction.EAST or Direction.WEST;
        }

        // Mostest correct would also use door direction, but coordinates is the only necessary information here.
        public override bool Equals(object obj) => obj is Door other && WorldCoordinates == other.WorldCoordinates;
        public bool Equals(Door other) => WorldCoordinates.Equals(other.WorldCoordinates);
        public static bool operator ==(Door left, Door right) => left.WorldCoordinates == right.WorldCoordinates;
        public static bool operator !=(Door left, Door right) => left.WorldCoordinates != right.WorldCoordinates;
        
        public override int GetHashCode() => HashCode.Combine(direction, GridCoordinates);
    }
}