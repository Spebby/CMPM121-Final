#if UNITY_EDITOR
using CMPM.MapGenerator;
using UnityEditor;
using UnityEngine;


namespace CMPM146.Editor {
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            MapGenerator t = (MapGenerator)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Map")) {
                t.Generate();
            }
        }
    }
}
#endif
