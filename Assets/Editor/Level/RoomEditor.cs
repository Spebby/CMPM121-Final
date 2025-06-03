#if UNITY_EDITOR
using CMPM.Level;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


namespace CMPM.Editor.Level {
    [CustomEditor(typeof(Room))]
    public class RoomEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            Room room = (Room)target;

            // Draw metadata
            #region Metadata
            EditorGUILayout.LabelField("Room Metadata", EditorStyles.boldLabel);

            room.ID = (uint)EditorGUILayout.IntField("Room ID", (int)room.ID);
            if (GUILayout.Button("Generate Random ID")) {
                room.ID = (uint)Random.Range(int.MinValue, int.MaxValue);
                EditorUtility.SetDirty(room);
            }

            room.Type  = (RoomType)EditorGUILayout.EnumPopup("Room Type", room.Type);
            room.Theme = (RoomTheme)EditorGUILayout.EnumPopup("Room Theme", room.Theme);

            room.Weight = EditorGUILayout.IntSlider("Spawn Weight", room.Weight, 0, 100);

            if (GUILayout.Button("Scan Spawn Points")) {
                // Your logic to scan and update room.Spawns
                room.ScanSpawns();
                EditorUtility.SetDirty(room);
                Debug.Log($"Found {room.Spawns?.Length ?? 0} spawn points.");
            }

            if (room.Spawns != null) {
                EditorGUILayout.LabelField("Spawn Points:");
                foreach (SpawnPoint spawn in room.Spawns) {
                    EditorGUILayout.LabelField($"- {spawn.transform} ({spawn.Kind})");
                }
            }

            if (GUILayout.Button("Scan Door Positions")) {
                // Your logic to scan and update room.Doors
                room.ScanDoors();
                EditorUtility.SetDirty(room);
                Debug.Log($"Found {room.Doors?.Length ?? 0} doors.");
            }

            if (room.Doors != null) {
                EditorGUILayout.LabelField("Doors:");
                foreach (DoorCords door in room.Doors) {
                    EditorGUILayout.LabelField($"- {door.Direction} @ {door.ToString()}");
                }
            }
            #endregion
           
            EditorGUILayout.Space(); 
            
            #region Room Sizers
            EditorGUILayout.LabelField("Room Grid", EditorStyles.boldLabel);
            
            int newWidth  = EditorGUILayout.IntField("Width", room.Width);
            int newHeight = EditorGUILayout.IntField("Height", room.Height);
            if (newWidth != room.Width || newHeight != room.Height) {
                Undo.RecordObject(room, "Resize Room Grid");
                room.ResizeGrid(newWidth, newHeight);
                //EditorUtility.SetDirty(room);
            }

            DrawOccupancyGrid(room);

            EditorGUILayout.Space(); 
            #endregion
            
            if (GUILayout.Button("Export to JSON")) {
                string json = JsonConvert.SerializeObject(room, Formatting.Indented);
                Debug.Log(json);
                // You could also write to disk here
            }
            
            // This is terrible but i don't care
            EditorUtility.SetDirty(room);
        }
        
        void DrawOccupancyGrid(Room room) {
            EditorGUILayout.LabelField("Tile Occupancy:");

            for (int y = 0; y < room.Height; y++) {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < room.Width; x++) {
                    int  index    = y * room.Width + x;
                    bool value    = room.Occupancy[index];
                    bool newValue = GUILayout.Toggle(value, "", GUILayout.Width(20));
                    
                    if (newValue == value) continue;
                    Undo.RecordObject(room, "Toggle Tile");
                    room.Occupancy[index] = newValue;
                    EditorUtility.SetDirty(room);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif
