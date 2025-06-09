#if UNITY_EDITOR
using CMPM146.MapGenerator;
using UnityEditor;
using UnityEngine;


namespace CMPM146.Editor {
    [CustomEditor(typeof(RoomArchetype))]
    public class RoomArchetypeEditor : UnityEditor.Editor {
        SerializedProperty _roomsProp;
        SerializedProperty _doorsProp;
        SerializedProperty _weightProp;
        SerializedProperty _widthProp;
        SerializedProperty _heightProp;

        void OnEnable() {
            _roomsProp     = serializedObject.FindProperty("rooms");
            _doorsProp     = serializedObject.FindProperty("doors");
            
            _weightProp    = serializedObject.FindProperty("Weight");
            _widthProp     = serializedObject.FindProperty("Width");
            _heightProp    = serializedObject.FindProperty("Height");
        }
       
        public override void OnInspectorGUI() {
            RoomArchetype archetype = (RoomArchetype)target;
            serializedObject.Update();

            EditorGUILayout.LabelField("Room Relations", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_roomsProp, true);
            EditorGUILayout.PropertyField(_doorsProp, true);
            
            EditorGUILayout.PropertyField(_weightProp);
            
            EditorGUILayout.LabelField("Room Grid", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_widthProp);
            EditorGUILayout.PropertyField(_heightProp);
            
            if (archetype.Width * archetype.Height != archetype.Occupancy.Length) {
                Undo.RecordObject(archetype, "Resize Room Grid");
                archetype.ResizeGrid(archetype.Width, archetype.Height);
                //EditorUtility.SetDirty(room);
            }
            DrawOccupancyGrid(archetype);

            serializedObject.ApplyModifiedProperties();
        }

        static void DrawOccupancyGrid(RoomArchetype archetype) {
            int    width     = archetype.Width;
            int    height    = archetype.Height;
            bool[] occupancy = archetype.Occupancy;
            
            EditorGUILayout.LabelField("Occupancy Grid:");
            for (int y = 0; y < height; y++) {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < width; x++) {
                    int index = y * width + x;
                    occupancy[index] = EditorGUILayout.Toggle(occupancy[index], GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorUtility.SetDirty(archetype);
        }
    }
}
#endif
