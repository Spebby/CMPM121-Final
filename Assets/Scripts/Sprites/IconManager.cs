using UnityEngine;
using UnityEngine.UI;


namespace CMPM.Sprites {
    public class IconManager : MonoBehaviour {
        [SerializeField] protected Sprite[] sprites;

        public void PlaceSprite(uint which, Image target) {
            target.sprite = sprites[which];
        }

        public Sprite Get(int index) {
            return sprites[index];
        }

        public int GetCount() {
            return sprites.Length;
        }
    }
}