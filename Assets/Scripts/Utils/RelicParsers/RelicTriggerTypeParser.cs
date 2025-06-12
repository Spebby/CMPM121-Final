using System;
using CMPM.Relics;
using Newtonsoft.Json;


namespace CMPM.Utils.RelicParsers {
    public class RelicTriggerTypeParser : JsonConverter<PreconditionType> {
        public override PreconditionType ReadJson(JsonReader reader, Type objectType, PreconditionType existingValue,
                                             bool hasExistingValue,
                                             JsonSerializer serializer) {
            string str = (reader.Value as string)?.ToLower();
            if (string.IsNullOrEmpty(str))
                throw new JsonReaderException("RelicTriggerTypeParser expected string 'Trigger.Type'!");

            return str switch {
                "on-kill"     => PreconditionType.OnKill,
                "on-hit"      => PreconditionType.OnHit,
                "stand-still" => PreconditionType.StandStill,
                "take-damage" => PreconditionType.TakeDamage,
                "timer"       => PreconditionType.Timer,
                "wave-start"  => PreconditionType.WaveStart,
                "room-clear"  => PreconditionType.RoomClear,
                "none"        => PreconditionType.None,
                _             => throw new NotImplementedException($"Unknown trigger type '{str}'")
            };
        }

        public override void WriteJson(JsonWriter writer, PreconditionType value, JsonSerializer serializer) {
            string str = value switch {
                PreconditionType.OnKill     => "on-kill",
                PreconditionType.OnHit      => "on-hit",
                PreconditionType.StandStill => "standing-still",
                PreconditionType.TakeDamage => "take-damage",
                PreconditionType.Timer      => "timer",
                PreconditionType.WaveStart  => "wave-start",
                PreconditionType.RoomClear  => "room-clear",
                PreconditionType.None       => "none",
                _                           => throw new NotImplementedException($"Unknown precondition type '{value}'")
            };

            writer.WriteValue(str);
        }
    }
}