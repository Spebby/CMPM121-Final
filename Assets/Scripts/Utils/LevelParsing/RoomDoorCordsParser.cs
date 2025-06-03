using System;
using CMPM.Level;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.LevelParsing {
    internal class RoomDoorCordsParser : JsonConverter<DoorCords> {
        public override DoorCords ReadJson(JsonReader reader, Type objectType, DoorCords existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            byte          x   = obj["x"]!.Value<byte>();
            byte          y   = obj["y"]!.Value<byte>();
            DoorDirection dir = Enum.Parse<DoorDirection>(obj["direction"]!.Value<string>()!, ignoreCase: true);

            return new DoorCords(x, y, (byte)dir);
        }

        public override void WriteJson(JsonWriter writer, DoorCords value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.X);
            writer.WritePropertyName("y");
            writer.WriteValue(value.Y);
            writer.WritePropertyName("direction");
            writer.WriteValue(value.Direction.ToString().ToLowerInvariant());
            writer.WriteEndObject();
        }
    }
}