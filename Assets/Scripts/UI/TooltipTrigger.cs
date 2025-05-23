using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


namespace CMPM.UI {
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] Tooltip tooltip;
        [SerializeField, Tooltip("Ensure 'Tooltip User' has a component that implements ITooltipUser.")] GameObject tooltipUser;
        ITooltipUser _user;
        bool _isHovering;
        
        void Start() {
            tooltip ??= GetComponent<Tooltip>();
            _user = tooltipUser ? tooltipUser.GetComponent<ITooltipUser>() : GetComponent<ITooltipUser>();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _isHovering = true;
            _user.ShowTooltip(tooltip);
        }
        
        public void OnPointerExit(PointerEventData eventData) {
            _isHovering = false;
            StartCoroutine(DelayedHide());
        }

        IEnumerator DelayedHide() {
            yield return new WaitForSeconds(0.1f);
            if (!_isHovering && !tooltip.IsHovering) {
                _user.HideTooltip();
            }
        } 
    }
}