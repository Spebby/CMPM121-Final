using System.Runtime.CompilerServices;
using CMPM.Utils.LevelParsing;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;


[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("CMPM.Utils.LevelParsing")]
namespace CMPM.Level {
    [JsonConverter(typeof(RoomDataParser))]
    public class Room : MonoBehaviour {
        #region Metadata
        [SerializeField] internal uint ID;
        [SerializeField] internal RoomType Type;
        [SerializeField] internal RoomTheme Theme;
        [SerializeField, Range(0, 100)] internal int Weight = 100;
        #endregion
       
        #region Room Data
        [SerializeField] internal DoorCords[] Doors;
        [SerializeField] [CanBeNull] internal SpawnPoint[] Spawns;
        
        [SerializeField] internal int Width = 2;
        [SerializeField] internal int Height = 2;
        [SerializeField] internal bool[] Occupancy = new bool[4];
        #endregion
        
        #region Spawns
        
        #endregion
        
        void Reset() {
            ResizeGrid(Width, Height);
        }
        
        internal void ResizeGrid(int newWidth, int newHeight) {
            if (newWidth == Width && newHeight == Height) return;

            bool[] newOccupancy = new bool[newWidth * newHeight];
            for (int y = 0; y < Mathf.Min(Height, newHeight); y++) {
                for (int x = 0; x < Mathf.Min(Width, newWidth); x++) {
                    int oldIndex = y * Width + x;
                    int newIndex = y * newWidth + x;
                    newOccupancy[newIndex] = Occupancy != null && oldIndex < Occupancy.Length && Occupancy[oldIndex];
                }
            }

            Width     = newWidth;
            Height    = newHeight;
            Occupancy = newOccupancy;
        }

        public void ScanSpawns() {
            throw new System.NotImplementedException();
        }

        public void ScanDoors() {
            throw new System.NotImplementedException();
        }
    }

    [JsonConverter(typeof(RoomTypeParser))]
    public enum RoomType {
        Start,
        Standard,
        Trap,
        Item,
        Open,
        Secret,
        Boss
    }

    [JsonConverter(typeof(RoomEnumParser))]
    public enum RoomTheme {
        Base
    }

    [JsonConverter(typeof(RoomDoorDirectionParser))]
    internal enum DoorDirection : byte {
        North = 0,
        South = 1,
        East  = 2,
        West  = 3
    }

    [JsonConverter(typeof(RoomDoorCordsParser))]
    internal readonly struct DoorCords {
        readonly ushort _packed;

        // [X:7][Y:7][Pos:2]
        public DoorCords(int x, int y, byte pos) {
            _packed = (ushort)(((x & 0x7F) << 9) | ((y & 0x7F) << 2) | (pos & 0b11));
        }

        public byte X => (byte)((_packed >> 9) & 0x7F);
        public byte Y => (byte)((_packed >> 2) & 0x7F);
        public DoorDirection Direction => (DoorDirection)(_packed & 0b11);

        public override string ToString() => $"<{X}, {Y}, {Direction}>";
    }

    [JsonConverter(typeof(RoomCordsParser))]
    internal readonly struct RoomCords {
        readonly ushort _packed;

        public RoomCords(byte x, byte y) {
            _packed = (ushort)(((x & 0x7F) << 8) | (y & 0x7F));
        }
        
        public byte X => (byte)(_packed >> 8);
        public byte Y => (byte)(_packed & 0xFF);
    }
    
    // I'm going to keep it bare for now until I can come up with more thing to put in here.
    [JsonConverter(typeof(RoomDataParser))]
    public readonly struct RoomData {
        public readonly uint ID;
        public readonly RoomType Type;
        public readonly RoomTheme Theme;

        internal readonly DoorCords[] Doors;
        internal readonly RoomCords[] Cords;
        internal readonly Spawn[] Spawns;
        
        internal RoomData(uint id, RoomType type, RoomTheme theme, in DoorCords[] doors, in RoomCords[] cords, in Spawn[] spawns) {
            ID     = id;
            Type   = type;
            Theme  = theme;
            Doors  = doors;
            Cords  = cords;
            Spawns = spawns;
        }

        // Item tables, points where chests can spawn, ground loot, etc
        
    }
}
