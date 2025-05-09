using System.Collections.Generic;
using CMPM.Core;
using CMPM.Spells;
using UnityEngine;


namespace CMPM.UI {
    public class SpellUIContainer : MonoBehaviour {
        public SpellUI[] spellUIs;
        public PlayerController player;

        void Awake() {
            spellUIs = GetComponentsInChildren<SpellUI>();
            
            foreach (SpellUI spellUI in spellUIs) {
                spellUI.highlight.SetActive(false);
            }
            spellUIs[0].highlight.SetActive(true);
        }
        
        public void SetSpellAsActive(int index) {
            int length = spellUIs.Length;
            if (length == 0) return;

            index = ((index % length) + length) % length;
            List<SpellUI> rotated = new(length);

            // visually rotate 
            for (int i = 0; i < length; ++i) {
                int rotatedIndex = (i + index) % length;
                rotated.Add(spellUIs[rotatedIndex]);
            }
            
            int siblingIndex = 0;
            foreach (SpellUI spell in rotated) {
                spell.highlight.SetActive(false);
                if (spell.IsEmpty()) {
                    spell.transform.SetAsLastSibling();
                    continue;
                }
                spell.transform.SetSiblingIndex(siblingIndex++);
            }

            rotated[0].highlight.SetActive(true);
        }
        
        // im too tired to do this right rn
        void FixedUpdate() {
            int activeCount = 0;
            foreach (SpellUI spellUI in spellUIs) {
                if (!spellUI.IsEmpty()) {
                    activeCount++;
                }

                if (activeCount >= spellUIs.Length) {
                    break;
                }
            }
            
            foreach (SpellUI spellUI in spellUIs) {
                if (!spellUI.IsEmpty()) {
                    spellUI.dropButton.SetActive(activeCount >= spellUIs.Length);
                }
            }
        }

        public void AddSpell(Spell spell, int i) {
            spellUIs[i].SetSpell(spell);
            if (i == 0) {
                spellUIs[0].highlight.SetActive(true);
            }
        }
    }
}