using System;
using CMPM.DamageSystem;
using CMPM.Spells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CMPM.Utils.SpellParsers {
    public class SpellDataParser : JsonConverter<SpellData> {
        public override SpellData ReadJson(JsonReader reader, Type objectType, SpellData existingValue,
                                           bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);

            // Metadata
            SpellData result = new() {
                Name        = obj["name"]?.ToString() ?? throw new NullReferenceException(),
                Description = obj["description"]?.ToString() ?? throw new NullReferenceException(),
                Icon        = obj["icon"]?.ToObject<uint>() ?? 0
            };

            // Damage (custom logic)
            if (obj["damage"] is JObject dmgObj) {
                string amount = dmgObj["amount"]?.ToString();
                string type   = dmgObj["type"]?.ToString();

                if (amount == null || type == null) {
                    throw new JsonException("Missing damage fields");
                }

                result.Damage = new SpellDamageData(
                    new RPNString(amount),
                    Damage.TypeFromString(type)
                );
            }

            // Stats (using serializer for known RPNString converters)
            result.ManaCost =
                serializer.Deserialize<RPNString>(obj["mana_cost"]?.CreateReader() ??
                                                  throw new Exception("Missing mana cost"));
            result.Cooldown =
                serializer.Deserialize<RPNString>(obj["cooldown"]?.CreateReader() ??
                                                  throw new Exception("Missing cooldown"));

            result.Projectile =
                serializer.Deserialize<ProjectileData>(obj["projectile"]?.CreateReader() ??
                                                       throw new Exception("Missing projectile"));
            if (obj.TryGetValue("secondary_projectile", out JToken secondary)) {
                result.SecondaryProjectile = serializer.Deserialize<ProjectileData>(secondary.CreateReader());
            }

            JToken countToken = obj["N"];
            result.Count = countToken != null && countToken.Type != JTokenType.Null
                ? new RPNString(countToken.ToString())
                : null;

            JToken sprayToken = obj["spray"];
            result.Spray = sprayToken != null && sprayToken.Type != JTokenType.Null
                ? new RPNString(sprayToken.ToString())
                : null;

            JToken slowFactorToken = obj["slow_factor"];
            result.SlowFactor = slowFactorToken != null && slowFactorToken.Type != JTokenType.Null
                ? slowFactorToken.ToObject<float>()
                : null;

            JToken timeSlowedToken = obj["time_slowed"];
            result.TimeSlowed = timeSlowedToken != null && timeSlowedToken.Type != JTokenType.Null
                ? timeSlowedToken.ToObject<int>()
                : null;

            return result;
        }

        public override void WriteJson(JsonWriter writer, SpellData value, JsonSerializer serializer) {
            // Metadata
            JObject obj = new() {
                ["name"]        = value.Name,
                ["description"] = value.Description,
                ["icon"]        = value.Icon
            };

            // Damage
            JObject dmg = new() {
                ["amount"] = value.Damage.DamageRPN.String,
                ["type"]   = value.Damage.Type.ToString().ToLowerInvariant()
            };
            obj["damage"] = dmg;

            // Stats
            obj["mana_cost"]  = JToken.FromObject(value.ManaCost, serializer);
            obj["cooldown"]   = JToken.FromObject(value.Cooldown, serializer);
            obj["projectile"] = JToken.FromObject(value.Projectile, serializer);
            if (value.SecondaryProjectile.HasValue) {
                obj["secondary_projectile"] = JToken.FromObject(value.SecondaryProjectile.Value, serializer);
            }

            if (value.Count != null) obj["N"]                = JToken.FromObject(value.Count, serializer);
            if (value.Spray != null) obj["spray"]            = JToken.FromObject(value.Spray, serializer);
            
            if (value.SlowFactor != null) obj["slow_factor"] = JToken.FromObject(value.SlowFactor, serializer);
            if (value.TimeSlowed != null) obj["time_slowed"] = JToken.FromObject(value.TimeSlowed, serializer);

            obj.WriteTo(writer);
        }
    }
}