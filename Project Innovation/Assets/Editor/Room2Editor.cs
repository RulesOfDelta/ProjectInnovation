using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room2))]
public class Room2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        var room = target as Room2;
        if (room == null) return;
        if (GUILayout.Button("Generate"))
        {
            room.EditModeGenerate();
        }

        if (GUILayout.Button("Clear"))
        {
            room.Clear();
        }
        base.OnInspectorGUI();
    }
}
