using System;
using System.Collections.Generic;
using CMPM.Level;
using CMPM.Utils.Structures;
using Newtonsoft.Json;


namespace CMPM.Utils.LevelParsing {
    public class SpawnEnemyParser : JsonConverter<EnemyData> {
        readonly SerializedDictionary<string, EnemyData> _enemies;

        public SpawnEnemyParser(in SerializedDictionary<string, EnemyData> enemies) {
            _enemies = enemies;
        }

        public override EnemyData ReadJson(JsonReader reader, Type objectType, EnemyData existingValue, bool hasExistingValue,
                                       JsonSerializer serializer) {
            string enemyName = reader.Value as string;
            if (string.IsNullOrEmpty(enemyName)) {
                throw new ArgumentException("Enemy name cannot be null or empty");
            }


            // Look up the enemy in the dictionary
            if (_enemies.TryGetValue(enemyName, out EnemyData enemy)) {
                return enemy;
            }

            throw new KeyNotFoundException($"Enemy '{enemyName}' not found in the enemies dictionary.");
        }

        public override void WriteJson(JsonWriter writer, EnemyData value, JsonSerializer serializer) {
            // Write back the enemy's name (if you need to serialize it later)
            writer.WriteValue(value.Name); // Assuming `Enemy` has a `name` field
        }
    }
}