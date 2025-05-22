using UnityEngine;


namespace CMPM.UI {
    public interface ITooltipUser {
        public void ShowTooltip(Tooltip tooltip, Vector3 pos);
        public void HideTooltip();
    }
}