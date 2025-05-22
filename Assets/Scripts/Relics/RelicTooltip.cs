using CMPM.UI;
using JetBrains.Annotations;
using UnityEngine;


namespace CMPM.Relics {
    public class RelicTooltip : Tooltip {
        protected void Show(Vector3 pos, string title, string desc, [CanBeNull] string trigger, [CanBeNull] string effect) {
            base.Show(pos, title, $"{desc}{(string.IsNullOrEmpty(trigger) || string.IsNullOrEmpty(effect) ? '\n' : "")}{(string.IsNullOrEmpty(trigger) ? '\n' : $"\nTrigger: {trigger}")}{(string.IsNullOrEmpty(effect) ? '\n' : $"\nEffect: {effect}")}");
        }
    }
}