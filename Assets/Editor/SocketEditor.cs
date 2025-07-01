using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Socket))]
public class SocketEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Socket socket = (Socket)target;

        if (GUILayout.Button("Auto Assign Socket Cubes"))
        {
            Undo.RecordObject(socket, "Auto Assign Cubes");
            socket.AutoAssignSocketCubes();
            EditorUtility.SetDirty(socket);
        }
    }
}
