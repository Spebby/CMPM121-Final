using UnityEngine;
using CMPM.Level;
using CMPM.Spells;
using CMPM.Relics;
using System;
using TMPro;


namespace CMPM.UI {
    public class LootGUI : MonoBehaviour {
        [SerializeField] GameObject spellUI;
        [SerializeField] GameObject relicIcon;

        [SerializeField] TextMeshProUGUI description;

        [SerializeField] LootController lootController;
        [SerializeField] GameObject GUI;

        void UpdateInfo() {
            Spell spell = lootController.FinalSpell;
            RelicData relic = lootController.RelicData;

            switch (lootController.type) {
                case LootController.LootType.SPELL:
                    spellUI.SetActive(true);
                    spellUI.GetComponent<SpellUI>().SetSpell(spell);
                    description.text = $"{spell.GetName()}\n{spell.GetDescription()}";
                    break;
                case LootController.LootType.RELIC:
                    relicIcon.SetActive(true);
                    relicIcon.GetComponent<RelicIcon>().Init(relic);
                    description.text = $"{relic.Name}\n{relic.GetFullDescription()}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnTriggerStay2D(Collider2D collision) {
            if (!collision.gameObject.CompareTag("Player")) return;
            UpdateInfo();
            GUI.SetActive(true);
        }

        void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Player")) {
                GUI.SetActive(false);
            }
        }
    }
}