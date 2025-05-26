using CMPM.UI;
using UnityEngine;
using UnityEditor;

namespace CMPM.Editor {
    [CustomEditor(typeof(RewardScreenManager))]
    public class RewardScreenManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Give Spell")) {
                RewardScreenManager myComponent = (RewardScreenManager)target;
                myComponent.DebugAddSpell();
            }
        }
    }
}