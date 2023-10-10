using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]
public class BoardInspector : Editor {
    const float toggleSize = 20f; // Adjust this value to set the size of the toggle fields
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        
        Board board = (Board) target;
        if (board.Blocks == null || board.Blocks.Length == 0) return;
    
        int width = board.Width;
        int height = board.Height;
    
        EditorGUILayout.LabelField("Block Grid IsActive");
    
        // Set the GUIStyle to remove margins and paddings
        GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);
        toggleStyle.margin = new RectOffset(0, 0, 0, 0);
        toggleStyle.padding = new RectOffset(0, 0, 0, 0);
    
        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
    
            for (int x = 0; x < width; x++) {
                
                Block block = board.Blocks[x, y];
    
                EditorGUIUtility.labelWidth = 0f; 
                bool isActive = EditorGUILayout.Toggle(block.IsActive, toggleStyle, GUILayout.Width(toggleSize),
                    GUILayout.Height(toggleSize));
    
                block.IsActive = isActive;
            }
    
            EditorGUILayout.EndHorizontal();
        }
    }
}