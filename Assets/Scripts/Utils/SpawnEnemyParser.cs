using System;
using System.Collections.Generic;
using CMPM.Level;
using Newtonsoft.Json;


namespace CMPM.Utils {
    public class SpawnEnemyParser : JsonConverter<Enemy> {
        readonly SerializedDictionary<string, Enemy> _enemies;
        public SpawnEnemyParser(in SerializedDictionary<string, Enemy> enemies) {
            _enemies = enemies;
        }
        
        public override Enemy ReadJson(JsonReader reader, Type objectType, Enemy existingValue, bool hasExistingValue, JsonSerializer serializer) {
            string enemyName = reader.Value as string;
            if (string.IsNullOrEmpty(enemyName)) {
                throw new ArgumentException("Enemy name cannot be null or empty");
            }
           
            
            // Look up the enemy in the dictionary
            if (_enemies.TryGetValue(enemyName, out Enemy enemy)) {
                return enemy;
            }

            throw new KeyNotFoundException($"Enemy '{enemyName}' not found in the enemies dictionary.");
        }

        public override void WriteJson(JsonWriter writer, Enemy value, JsonSerializer serializer) {
            // Write back the enemy's name (if you need to serialize it later)
            writer.WriteValue(value.name); // Assuming `Enemy` has a `name` field
        }
    }
}