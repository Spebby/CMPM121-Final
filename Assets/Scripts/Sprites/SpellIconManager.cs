using CMPM.Core;


namespace CMPM.Sprites {
    public class SpellIconManager : IconManager {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            GameManager.Instance.SpellIconManager = this;
        }
    }
}