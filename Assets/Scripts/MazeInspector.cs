using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeBuilder))]
public class LabyrinthInspector : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MazeBuilder script = (MazeBuilder)target;
        if(GUILayout.Button("Generate")){
            script.clearMaze();
            script.generateMaze();
        }
    }
}