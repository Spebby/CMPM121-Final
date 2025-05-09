using System;
using System.Collections.Generic;
using CMPM.Core;
using UnityEngine;


namespace CMPM.UI {
    public class SpellUIContainer : MonoBehaviour {
        public SpellUI[] spellUIs;
        public PlayerController player;

        void Awake() {
            spellUIs = GetComponentsInChildren<SpellUI>();
        }

        void Start() {
            // we only have one spell (right now)
            spellUIs[0].gameObject.SetActive(true);
            for (int i = 1; i < spellUIs.Length; ++i) {
                spellUIs[i].gameObject.SetActive(false);
            }
        }
        
        // im too tired to do this right rn
        void FixedUpdate() {
            int activeCount = 0;
            foreach (SpellUI spellUI in spellUIs) {
                if (spellUI.gameObject.activeInHierarchy) {
                    activeCount++;
                }

                if (activeCount >= 2) {
                    break;
                }
            }

            foreach (SpellUI spellUI in spellUIs) {
                if (spellUI.gameObject.activeInHierarchy) {
                    spellUI.dropButton.SetActive(activeCount >= 2);
                }
            }
        }
    }
}