using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameCore))]
public class EditorGameCore : Editor
{
    public override void OnInspectorGUI()
    {
        GameCore levelGen = (GameCore)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Generate Level"))
        {
            levelGen.GenerateWorld();
        }

        if(GUILayout.Button("Delete Level"))
        {
            levelGen.DeleteLevel();
        }
    }
}