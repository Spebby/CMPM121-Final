using System;
using System.Collections.Generic;
using CMPM.Spells;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMPM.Utils {
    public class SpellModifierDataParser : JsonConverter<SpellModifierData> {
        public override SpellModifierData ReadJson(JsonReader reader, Type objectType, SpellModifierData existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject obj         = JObject.Load(reader);
            string  name        = obj.Value<string>("name") ?? throw new JsonException("Name is required!");
            string  description = obj.Value<string>("description") ?? "";

            string    damageMultiplier = obj.Value<string>("damage_multiplier") ?? "1";
            string    damageAdder      = obj.Value<string>("damage_adder") ?? "";
            RPNString damageModifier   = new($"value {damageMultiplier} * {damageAdder} +");

            string    manaCostMultiplier = obj.Value<string>("mana_multiplier") ?? "1";
            string    manaCostAdder      = obj.Value<string>("mana_adder") ?? "0";
            RPNString manaModifier       = new($"value {manaCostMultiplier} * {manaCostAdder} +");

            string    cooldownMultiplier = obj.Value<string>("cooldown_multiplier") ?? "1";
            string    cooldownAdder      = obj.Value<string>("cooldown_adder") ?? "0";
            RPNString cooldownModifier   = new($"value {cooldownMultiplier} * {cooldownAdder} +");

            string    speedMultiplier = obj.Value<string>("speed_multiplier") ?? "1";
            string    speedAdder      = obj.Value<string>("speed_adder") ?? "0";
            RPNString speedModifier   = new($"value {speedMultiplier} * {speedAdder} +");

            string    lifetimeMultiplier = obj.Value<string>("lifetime_multiplier") ?? "1";
            string    lifetimeAdder      = obj.Value<string>("lifetime_adder") ?? "0";
            RPNString lifetimeModifier   = new($"value {lifetimeMultiplier} * {lifetimeAdder} +");

            RPNString count = new(obj.Value<string>("count") ?? "1");
            RPNString angle = new(obj.Value<string>("angle") ?? "0");
            RPNString delay = new(obj.Value<string>("delay") ?? "0");

            string typeStr = obj.Value<string>("projectile_trajectory");
            ProjectileType? type = string.IsNullOrEmpty(typeStr)
                ? null
                : ProjectileManager.StringToProjectileType(typeStr);
                

            return new SpellModifierData(
                name,
                description,
                damageModifier,
                manaModifier,
                speedModifier,
                cooldownModifier,
                lifetimeModifier,
                type,
                angle,
                delay,
                count
            );
        }
        
        public override void WriteJson(JsonWriter writer, SpellModifierData value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}