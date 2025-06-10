using UnityEngine;
using UnityEngine.UI;


namespace CMPM.Sprites {
    public class IconManager : MonoBehaviour {
        [SerializeField] protected Sprite[] sprites;

        public void PlaceSprite(uint which, Image target) {
            target.sprite = sprites[which];
        }

        public void PlaceSprite(uint which, SpriteRenderer target) {
            target.sprite = sprites[which];
        }

        public Sprite Get(uint index) => sprites[index];
        public Sprite Get(int index) => sprites[index];
        public int GetCount() => sprites.Length;
    }
}