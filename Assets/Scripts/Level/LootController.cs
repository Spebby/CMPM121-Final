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
        public enum ItemType
        {
            SPELL,
            RELIC
        }
        public ItemType _type;

        public enum ItemRarity
        {
            COMMON,
            UNCOMMON,
            RARE
        }
        public ItemRarity _rarity;

        public string _name;
        public uint _icon;

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
                case ItemType.SPELL:
                    pc.AddSpell(finalSpell);
                    break;

                case ItemType.RELIC:
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
                case ItemType.SPELL:
                    spellData = SpellFromRarity(_rarity);
                    _name = spellData.Name;
                    _icon = spellData.Icon;
                    GameManager.Instance.SpellIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    finalSpell = SpellBuilder.BuildSpell(_name, pc, 3);
                    break;

                case ItemType.RELIC:
                    relicData = RelicFromRarity(_rarity);
                    _name = relicData.Name;
                    _icon = relicData.Sprite;
                    GameManager.Instance.RelicIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    finalRelic = RelicBuilder.CreateRelic(relicData);
                    break;
            }
        }


        RelicData RelicFromRarity(ItemRarity input)
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

        SpellData SpellFromRarity(ItemRarity input)
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

        ItemRarity RandomRarity()
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

        public static ItemRarity RarityFromString(string input)
        {
            return input.ToLower() switch
            {
                "common" => ItemRarity.COMMON,
                "uncommon" => ItemRarity.UNCOMMON,
                "rare" => ItemRarity.RARE,
                _ => throw new ArgumentException($"'{input}' aint a real rarity")
            };
        }

        public static string StringFromRarity(ItemRarity input)
        {
          return input switch
            {
                ItemRarity.COMMON => "common",
                ItemRarity.UNCOMMON => "uncommon",
                ItemRarity.RARE => "rare",
                _ => throw new ArgumentException($"'{input}' aint a real rarity")
            };  
        }
    }
}