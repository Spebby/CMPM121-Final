#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace CMPM146.Editor {
    [CustomEditor(typeof(MapGenerator.MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            MapGenerator.MapGenerator t = (MapGenerator.MapGenerator)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Map")) {
                t.Generate();
            }
        }
    }
}
#endif
