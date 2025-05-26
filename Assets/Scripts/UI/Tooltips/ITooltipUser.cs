namespace CMPM.UI.Tooltips {
    public interface ITooltipUser {
        public void ShowTooltip(Tooltip tooltip);
        public void HideTooltip();
        public bool IsHovering();
    }
}