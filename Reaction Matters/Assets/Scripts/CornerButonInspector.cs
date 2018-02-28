using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CornerButton))]
 
public class CornerButonInspector : Editor
{

        public override void OnInspectorGUI()
        {
        CornerButton build = (CornerButton)target;
        if (GUILayout.Button("In Corner"))
        {
            build.CreateIn();
        }
        if (GUILayout.Button("Out Corner"))
        {
            build.CreateOut();
        }
        if (GUILayout.Button("No Corner"))
        {
            build.DeleteSelf();
        }
        build.ShowInputs = EditorGUILayout.Toggle("Show Default Inputs", build.ShowInputs);
        if (build.ShowInputs)
        {
            DrawDefaultInspector();
        }
    }

	
	
}
