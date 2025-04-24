using CMPM.Core;


namespace CMPM.Sprites {
    public class PlayerSpriteManager : IconManager {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            GameManager.INSTANCE.PlayerSpriteManager = this;
        }
    }
}