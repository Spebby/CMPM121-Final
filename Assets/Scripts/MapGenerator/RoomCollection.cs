using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
namespace CMPM.MapGenerator {
    [CreateAssetMenu(fileName = "RoomCollection", menuName = "MapGenerator/RoomCollection")]
    public class RoomCollection : ScriptableObject {
        public RoomArchetype[] rooms;
        
        public RoomArchetype Get(int i) => rooms[i];
        
        public RoomArchetype GetRandomRoom(in System.Random rng) {
            return GetRandomRoom(rooms, rng);
        }

        public RoomArchetype GetRandomRoomForDirection(in System.Random rng, Door.Direction direction) {
            return GetRandomRoom(rooms.Where(arch => arch.HasDoorOnSide(direction)).ToArray(), rng);
        }

        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        static RoomArchetype GetRandomRoom(in RoomArchetype[] collection, in System.Random rng) {
            int totalWeight = collection.Sum(room => room.Weight);
            if (totalWeight == 0) throw new InvalidOperationException("All room weights are zero.");

            int pick  = rng.Next(totalWeight);
            int accum = 0;

            for (int i = 0; i < collection.Length; i++) {
                RoomArchetype room = collection[i];
                accum += room.Weight;
                if (pick < accum) return room;
            }

            return collection[^1];
        }
    }
}