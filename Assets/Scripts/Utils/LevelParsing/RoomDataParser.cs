using System;
using CMPM.Level;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.LevelParsing {
    public class RoomDataParser : JsonConverter<RoomData> {
        public override RoomData ReadJson(JsonReader reader, Type objectType, RoomData existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            uint      id    = obj["ID"]!.Value<uint>();
            RoomType  type  = obj["type"]!.ToObject<RoomType>(serializer)!;
            RoomTheme theme = obj["theme"]!.ToObject<RoomTheme>(serializer)!;

            RoomCords[] shape = obj["shape"]!.ToObject<RoomCords[]>(serializer)!;
            DoorCords[] doors = obj["doors"]!.ToObject<DoorCords[]>(serializer)!;

            JToken  spawnsToken = obj["spawns"];
            Spawn[] spawnsArray = spawnsToken?.ToObject<Spawn[]>(serializer);
            
            return new RoomData(id, type, theme, in doors, in shape, in spawnsArray);
        }

        public override void WriteJson(JsonWriter writer, RoomData value, JsonSerializer serializer) {
            throw new NotImplementedException("Serialisation not implemented yet.");
        }
    }
 
}