using UnityEditor;

[CustomEditor(typeof(Board))]
public class MultidimensionalArrayInspector : Editor {
    // public override void OnInspectorGUI() {
    //     DrawDefaultInspector();
    //
    //     Board board = (Board) target;
    //
    //     EditorGUILayout.LabelField("Board");
    //
    //     int numRows = board.grid.GetLength(0);
    //     int numCols = board.grid.GetLength(1);
    //
    //     for (int col = numCols - 1; col >= 0; col--) {
    //         EditorGUILayout.BeginHorizontal();
    //
    //         for (int row = 0; row < numRows; row++) {
    //             board.grid[row, col] = EditorGUILayout.IntField(board.grid[row, col]);
    //         }
    //
    //         EditorGUILayout.EndHorizontal();
    //     }
    // }
}