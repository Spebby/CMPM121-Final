using UnityEngine;
using CMPM.Level;
using CMPM.Spells;
using CMPM.Relics;
using System;
using TMPro;

namespace CMPM.UI
{
    public class LootGUI : MonoBehaviour
    {
        [SerializeField] GameObject spellUI;
        [SerializeField] GameObject relicIcon;

        [SerializeField] TextMeshProUGUI description;

        [SerializeField] LootController lootController;

        void UpdateInfo()
        {
            Spell _spell = lootController.finalSpell;
            Relic _relic = lootController.finalRelic;

            if (_relic == null)
            {
                spellUI.SetActive(true);
                spellUI.GetComponent<SpellUI>().SetSpell(_spell);
                description.text = $"{_spell.GetName()}\n{_spell.GetDescription()}";
            }
            else
            {
                relicIcon.SetActive(true);
                relicIcon.GetComponent<RelicIcon>().Init(_relic);
                description.text = $"{_relic.Name}\n{_relic.Description}";
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.F))
                {
                    UpdateInfo();
                    transform.Find("GUI").gameObject.SetActive(true);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                transform.Find("GUI").gameObject.SetActive(false);
            }
        }

        void Awake()
        {
            // glint
            // the PickupItem gameobject has children that contain particle systems
            // that are all off by default
            GameObject glint = lootController._rarity switch
            {
                LootController.LootRarity.COMMON => transform.Find("Common Glint").gameObject,
                LootController.LootRarity.UNCOMMON => transform.Find("Uncommon Glint").gameObject,
                LootController.LootRarity.RARE => transform.Find("Rare Glint").gameObject,
                _ => throw new Exception($"'{lootController._rarity}' isn't a valid rarity type")
            };
            glint.SetActive(true);   
        }
    }
}