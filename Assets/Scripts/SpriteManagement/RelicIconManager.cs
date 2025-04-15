public class RelicIconManager : IconManager {
    void Start() {
        GameManager.Instance.RelicIconManager = this;
    }
}