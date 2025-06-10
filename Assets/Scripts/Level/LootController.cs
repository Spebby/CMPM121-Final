using UnityEngine;
using System;
using CMPM.Spells;
using CMPM.Core;
using CMPM.Relics;
using System.Collections.Generic;

namespace CMPM.Level
{
    public class LootController : MonoBehaviour
    {
        // if we still have time, i lowkey want to make recovery items
        public enum LootType
        {
            SPELL,
            RELIC
        }
        public LootType _type;

        public enum LootRarity
        {
            COMMON,
            UNCOMMON,
            RARE
        }
        public LootRarity _rarity;

        string _name;
        uint _icon;

        SpellData spellData;
        public Spell finalSpell;
        RelicData relicData;
        public Relic finalRelic;

        PlayerController pc;

        // added this to the accept button in Unity Inspector
        public void PickupItem()
        {
            switch (_type)
            {
                case LootType.SPELL:
                    pc.AddSpell(finalSpell);
                    break;

                case LootType.RELIC:
                    pc.AddRelic(finalRelic);
                    break;
            }
        }

        public void Forget()
        {
            Destroy(gameObject);
        }

        void Awake()
        {
            pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _rarity = RandomRarity();

            switch (_type)
            {
                case LootType.SPELL:
                    spellData = SpellFromRarity(_rarity);
                    _name = spellData.Name;
                    _icon = spellData.Icon;
                    GameManager.Instance.SpellIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    finalSpell = SpellBuilder.BuildSpell(_name, pc, 3);
                    break;

                case LootType.RELIC:
                    relicData = RelicFromRarity(_rarity);
                    _name = relicData.Name;
                    _icon = relicData.Sprite;
                    GameManager.Instance.RelicIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    finalRelic = RelicBuilder.CreateRelic(relicData);
                    break;
            }
        }

        RelicData RelicFromRarity(LootRarity input)
        {
            List<RelicData> pool = new();
            for (int i = 0; i < RelicRegistry.Count; i++)
            {
                RelicData relic = RelicRegistry.Get(i);
                if (relic.Rarity == StringFromRarity(input))
                {
                    pool.Add(relic);
                }
            }

            if (pool.Count == 0) throw new Exception($"No relic found of rarity: '{input}'");
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }

        SpellData SpellFromRarity(LootRarity input)
        {
            List<SpellData> pool = new();
            foreach (var hash in SpellRegistry.GetHashes())
            {
                SpellData spell = SpellRegistry.Get(hash);
                if (spell.Rarity == StringFromRarity(input))
                {
                    pool.Add(spell);
                }
            }
            
            if (pool.Count == 0) throw new Exception($"No spell found of rarity: '{input}'");
            return pool[UnityEngine.Random.Range(0, pool.Count)];
        }

        LootRarity RandomRarity()
        {
            int randomNumber = UnityEngine.Random.Range(1, 101);
            foreach (var weight in GameManager.Instance.LootWeights)
            {
                if (randomNumber <= weight.Value)
                {
                    return RarityFromString(weight.Key);
                }
                else
                {
                    randomNumber -= weight.Value;
                }
            }
            throw new Exception("cuz nOt AlL cOdE pAtHs ReTurN a VaLuE");
        }

        public static LootRarity RarityFromString(string input)
        {
            return input.ToLower() switch
            {
                "common" => LootRarity.COMMON,
                "uncommon" => LootRarity.UNCOMMON,
                "rare" => LootRarity.RARE,
                _ => throw new ArgumentException($"'{input}' aint a real rarity")
            };
        }

        public static string StringFromRarity(LootRarity input)
        {
          return input switch
            {
                LootRarity.COMMON => "common",
                LootRarity.UNCOMMON => "uncommon",
                LootRarity.RARE => "rare",
                _ => throw new ArgumentException($"'{input}' aint a real rarity")
            };  
        }
    }
}