using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorInspector : Editor
{
   
    public override void OnInspectorGUI()
    {
        LevelEditor gui = (LevelEditor)target;
        EditorGUILayout.PrefixLabel("Level Properties");
        gui.SizeX = EditorGUILayout.IntField("Size X", gui.SizeX);
        gui.SizeZ = EditorGUILayout.IntField("Size Y", gui.SizeZ);

        LevelEditor build = (LevelEditor)target;
        if (GUILayout.Button("Build Grid"))
        {
            build.BuildGrid();
        }
        if (GUILayout.Button("Undo"))
        {
            build.Undo();
        }
        if (GUILayout.Button("Finish"))
        {
            build.Finish();
        }
        gui.MakeWall = EditorGUILayout.Toggle("Build Walls", gui.MakeWall);

        EditorGUILayout.PrefixLabel(" ");
        gui.ShowInputs = EditorGUILayout.Toggle("Show Default Inputs", gui.ShowInputs);
        if(gui.ShowInputs)
        {
            DrawDefaultInspector();
        }

    }


}
