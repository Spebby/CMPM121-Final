using System;
using CMPM.Level;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.LevelParsing {
    internal class RoomCordsParser : JsonConverter<RoomCords> {
        public override RoomCords ReadJson(JsonReader reader, Type objectType, RoomCords existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject  obj = JObject.Load(reader);
            byte x   = obj["x"]!.Value<byte>();
            byte y   = obj["y"]!.Value<byte>();
            return new RoomCords(x, y);
        }

        public override void WriteJson(JsonWriter writer, RoomCords value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.X);
            writer.WritePropertyName("y");
            writer.WriteValue(value.Y);
            writer.WriteEndObject();
        }
    }
}