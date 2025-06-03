using System;
using CMPM.Level;
using Newtonsoft.Json;


namespace CMPM.Utils.LevelParsing {
    public class RoomEnumParser : JsonConverter<RoomTheme> {
        public override RoomTheme ReadJson(JsonReader reader, Type objectType, RoomTheme existingValue, bool hasExistingValue, JsonSerializer serializer) {
            string str = (reader.Value as string)!;
            return Enum.Parse<RoomTheme>(str, ignoreCase: true);
        }

        public override void WriteJson(JsonWriter writer, RoomTheme value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
    
    public class RoomTypeParser : JsonConverter<RoomType> {
        public override RoomType ReadJson(JsonReader reader, Type objectType, RoomType existingValue, bool hasExistingValue, JsonSerializer serializer) {
            string str = (reader.Value as string)!;
            return Enum.Parse<RoomType>(str, ignoreCase: true);
        }

        public override void WriteJson(JsonWriter writer, RoomType value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
    
    internal class RoomDoorDirectionParser : JsonConverter<DoorDirection> {
        public override DoorDirection ReadJson(JsonReader reader, Type objectType, DoorDirection existingValue, bool hasExistingValue, JsonSerializer serializer) {
            string str = (reader.Value as string)!;
            return Enum.Parse<DoorDirection>(str, ignoreCase: true);
        }

        public override void WriteJson(JsonWriter writer, DoorDirection value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}