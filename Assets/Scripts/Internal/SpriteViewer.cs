using System.Collections;
using System.Collections.Generic;
using CMPM.Core;
using UnityEngine;


namespace CMPM.Internal {
    public class SpriteViewer : MonoBehaviour {
        public enum Mode {
            SPELLICONS,
            ENEMIES,
            PROJECTILES,
            CLASSES
        }

        public GameObject spriteView;
        int _perRow;

        List<GameObject> _views;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            _perRow = (Screen.width - 40) / 80;
            _views  = new List<GameObject>();
            StartCoroutine(ShowView());
        }

        IEnumerator ShowView() {
            yield return new WaitForSeconds(0.1f);
            ChangeMode("enemies");
        }

        public void ChangeMode(string m) {
            foreach (GameObject go in _views) {
                Destroy(go);
            }

            _views.Clear();
            switch (m) {
                case "spellicons": {
                    for (int i = 0; i < GameManager.Instance.SpellIconManager.GetCount(); ++i) {
                        GameObject newSv = Instantiate(spriteView, transform);
                        int        x     = i % _perRow;
                        int        y     = i / _perRow;

                        newSv.transform.Translate(x * 80, -y * 100, 0, Space.Self);
                        newSv.GetComponent<SpriteView>()
                             .Apply(i.ToString(), GameManager.Instance.SpellIconManager.Get(i));
                        _views.Add(newSv);
                    }

                    break;
                }
                case "enemies": {
                    for (int i = 0; i < GameManager.Instance.EnemySpriteManager.GetCount(); ++i) {
                        GameObject newSv = Instantiate(spriteView, transform);
                        int        x     = i % _perRow;
                        int        y     = i / _perRow;
                        newSv.transform.Translate(x * 80, -y * 100, 0, Space.Self);
                        newSv.GetComponent<SpriteView>()
                             .Apply(i.ToString(), GameManager.Instance.EnemySpriteManager.Get(i));
                        _views.Add(newSv);
                    }

                    break;
                }
                case "relics": {
                    for (int i = 0; i < GameManager.Instance.RelicIconManager.GetCount(); ++i) {
                        GameObject newSv = Instantiate(spriteView, transform);
                        int        x     = i % _perRow;
                        int        y     = i / _perRow;
                        newSv.transform.Translate(x * 80, -y * 100, 0, Space.Self);
                        newSv.GetComponent<SpriteView>()
                             .Apply(i.ToString(), GameManager.Instance.RelicIconManager.Get(i));
                        _views.Add(newSv);
                    }

                    break;
                }
                case "player": {
                    for (int i = 0; i < GameManager.Instance.PlayerSpriteManager.GetCount(); ++i) {
                        GameObject newSv = Instantiate(spriteView, transform);
                        int        x     = i % _perRow;
                        int        y     = i / _perRow;
                        newSv.transform.Translate(x * 80, -y * 100, 0, Space.Self);
                        newSv.GetComponent<SpriteView>()
                             .Apply(i.ToString(), GameManager.Instance.PlayerSpriteManager.Get(i));
                        _views.Add(newSv);
                    }

                    break;
                }
            }
        }
    }
}