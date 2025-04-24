using CMPM.Core;


namespace CMPM.Sprites {
    public class EnemySpriteManager : IconManager {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            GameManager.INSTANCE.EnemySpriteManager = this;
        }

        // Update is called once per frame
        void Update() { }
    }
}