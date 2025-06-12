using UnityEngine;
using System;
using System.Collections;
using CMPM.Spells;
using CMPM.Core;
using CMPM.Relics;
using System.Collections.Generic;
using UnityEngine.Serialization;


namespace CMPM.Level {
    public class LootController : MonoBehaviour {
        // if we still have time, i lowkey want to make recovery items
        public enum LootType {
            SPELL,
            RELIC
        }

        [FormerlySerializedAs("_type")] public LootType type;

        public enum LootRarity {
            COMMON,
            UNCOMMON,
            RARE
        }

        [FormerlySerializedAs("_rarity")] public LootRarity rarity;

        string _name;
        uint _icon;

        public SpellData SpellData { get; private set; }
        public RelicData RelicData { get; private set; }
        public Spell FinalSpell { get; private set; }
        Func<Relic> _finalRelic;

        PlayerController _player;

        // added this to the accept button in Unity Inspector
        public void PickupItem() {
            switch (type) {
                case LootType.SPELL:
                    _player.AddSpell(FinalSpell);
                    break;

                case LootType.RELIC:
                    _player.AddRelic(_finalRelic());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Forget() {
            Destroy(gameObject);
        }

        void Awake() {
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            rarity  = RandomRarity();
            SetSparkle(rarity);

            switch (type) {
                case LootType.SPELL:
                    SpellData = SpellFromRarity(rarity);
                    _name     = SpellData.Name;
                    _icon     = SpellData.Icon;
                    GameManager.Instance.SpellIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    FinalSpell = SpellBuilder.BuildSpell(_name, _player, 3);
                    break;

                case LootType.RELIC:
                    RelicData = RelicFromRarity(rarity);
                    _name     = RelicData.Name;
                    _icon     = RelicData.Sprite;
                    GameManager.Instance.RelicIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    // If you build a relic it starts timers *immediately*. Delay its construction until player picks it.
                    _finalRelic = () => {
                        BitArray set = _player.RelicOwnership;
                        set.Set(RelicRegistry.GetIndexFromRelic(RelicData), true);
                        return RelicBuilder.CreateRelic(RelicData);
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        RelicData RelicFromRarity(LootRarity input) {
            BitArray        ownership = _player.RelicOwnership;
            List<RelicData> pool      = new();
            for (int i = 0; i < RelicRegistry.Count; i++) {
                RelicData relic = RelicRegistry.Get(i);
                if (!ownership[i] && relic.Rarity == StringFromRarity(input)) {
                    pool.Add(relic);
                }
            }

            if (pool.Count == 0) {
                Debug.LogWarning($"No relic found of rarity: '{input}', using fallback.");
                pool.Add(RelicRegistry.Get(0));
                SetSparkle(RarityFromString(pool[0].Rarity));
            }

            if (pool.Count == 0) throw new Exception($"No relic found of rarity: '{input}'");
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }

        SpellData SpellFromRarity(LootRarity input) {
            List<SpellData> pool = new();
            foreach (int hash in SpellRegistry.GetHashes()) {
                SpellData spell = SpellRegistry.Get(hash);
                if (spell.Rarity == StringFromRarity(input)) {
                    pool.Add(spell);
                }
            }

            if (pool.Count == 0) throw new Exception($"No spell found of rarity: '{input}'");
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }

        LootRarity RandomRarity() {
            int randomNumber = UnityEngine.Random.Range(1, 101);
            foreach (KeyValuePair<string, int> weight in GameManager.Instance.LootWeights) {
                if (randomNumber <= weight.Value) {
                    return RarityFromString(weight.Key);
                }

                randomNumber -= weight.Value;
            }

            throw new Exception("cuz nOt AlL cOdE pAtHs ReTurN a VaLuE");
        }

        public static LootRarity RarityFromString(string input) {
            return input.ToLower() switch {
                "common"   => LootRarity.COMMON,
                "uncommon" => LootRarity.UNCOMMON,
                "rare"     => LootRarity.RARE,
                _          => throw new ArgumentException($"invalid rarity: {input}")
            };
        }

        public static string StringFromRarity(LootRarity input) {
            return input switch {
                LootRarity.COMMON   => "common",
                LootRarity.UNCOMMON => "uncommon",
                LootRarity.RARE     => "rare",
                _                   => throw new ArgumentException($"Invalid rarity: {input}")
            };
        }
        
        
        void SetSparkle(LootRarity r) {
            GameObject glint = r switch {
                LootRarity.COMMON   => transform.Find("Common Glint").gameObject,
                LootRarity.UNCOMMON => transform.Find("Uncommon Glint").gameObject,
                LootRarity.RARE     => transform.Find("Rare Glint").gameObject,
                _                                  => throw new Exception($"'{r}' isn't a valid rarity type")
            };
            glint.SetActive(true);
        }
    }
}