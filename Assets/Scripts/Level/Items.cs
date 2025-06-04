using UnityEngine;
using System;
using CMPM.Spells;
using CMPM.Core;
using CMPM.Relics;
using System.Collections.Generic;

namespace CMPM.Level
{
    public class Item : MonoBehaviour
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
        RelicData relicData;

        PlayerController pc;

        public void PickupItem()
        {
            switch (_type)
            {
                case ItemType.SPELL:
                    pc.AddSpell(SpellBuilder.BuildSpell(_name, pc, 3));
                    break;

                case ItemType.RELIC:
                    pc.AddRelic(RelicBuilder.CreateRelic(relicData));
                    break;
            }
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
                    break;

                case ItemType.RELIC:
                    relicData = RelicRegistry.GetRandom();
                    _name = relicData.Name;
                    _icon = relicData.Sprite;
                    _rarity = RarityFromString(relicData.Rarity);
                    GameManager.Instance.RelicIconManager.PlaceSprite(_icon, GetComponent<SpriteRenderer>());
                    break;
            }

            // glint
            // the PickupItem gameobject has children that contain particle systems
            // that are all off by default
            GameObject glint = _rarity switch
            {
                ItemRarity.COMMON => transform.Find("Common Glint").gameObject,
                ItemRarity.UNCOMMON => transform.Find("Uncommon Glint").gameObject,
                ItemRarity.RARE => transform.Find("Rare Glint").gameObject,
                _ => throw new Exception($"'{_name}' doesn't have a rarity: '{_rarity}'")
            };
            glint.SetActive(true);
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
            throw new Exception("balls itch");
        }

        public ItemRarity RarityFromString(string input)
        {
            return input switch
            {
                "common" => ItemRarity.COMMON,
                "uncommon" => ItemRarity.UNCOMMON,
                "rare" => ItemRarity.RARE,
                _ => throw new ArgumentException($"'{_name}' has unknown rarity type: '{input}'")
            };
        }

        public string StringFromRarity(ItemRarity input)
        {
          return input switch
            {
                ItemRarity.COMMON => "common",
                ItemRarity.UNCOMMON => "uncommon",
                ItemRarity.RARE => "rare",
                _ => throw new ArgumentException($"'{input}' aint a real rarity or wrong type")
            };  
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.F))
                {
                    // open gui w/ full list of modifiers
                    // then call PickupItem() when the player
                    // presses an "Accept" button
                    PickupItem();
                    Destroy(gameObject);
                }
            }
        }
    }
}