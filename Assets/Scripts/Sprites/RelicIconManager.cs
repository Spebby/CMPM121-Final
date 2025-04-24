using CMPM.Core;


namespace CMPM.Sprites {
    public class RelicIconManager : IconManager {
        void Start() {
            GameManager.INSTANCE.RelicIconManager = this;
        }
    }
}