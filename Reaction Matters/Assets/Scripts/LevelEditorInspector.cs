using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
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
        if (GUILayout.Button("Add Corners"))
        {
            build.Cornerfy();
        }
        if (GUILayout.Button("Clean Corners"))
        {
            build.ClearCorners();
        }
        if (GUILayout.Button("Show Corners"))
        {
            build.ActiveCorners();
        }
        if (GUILayout.Button("ReBuild Grid"))
        {
            build.Rebuild();
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
#endif