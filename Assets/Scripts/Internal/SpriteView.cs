using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CMPM.Internal {
    public class SpriteView : MonoBehaviour {
        public TextMeshProUGUI label;

        public Image image;

        public void Apply(string newLabel, Sprite sprite) {
            label.text   = newLabel;
            image.sprite = sprite;
        }
    }
}