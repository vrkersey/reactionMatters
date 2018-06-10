using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(BezierCurve))]
public class CurveInspector : Editor
{
    public override void OnInspectorGUI()
    {
        BezierCurve build = (BezierCurve)target;
        build.StartNode.Position = (GameObject)EditorGUILayout.ObjectField("StartNode", build.StartNode.Position, typeof(GameObject), true);
        build.StartNode.Direction = (GameObject)EditorGUILayout.ObjectField("StartDirectionNode", build.StartNode.Direction, typeof(GameObject), true);
        build.EndNode.Position = (GameObject)EditorGUILayout.ObjectField("EndNode", build.EndNode.Position, typeof(GameObject), true);
        build.EndNode.Direction = (GameObject)EditorGUILayout.ObjectField("EndDirectionNode", build.EndNode.Direction, typeof(GameObject), true);
        if (GUILayout.Button("Build Grid"))
        {
            build.GenertatePoints();
            
        }
        DrawDefaultInspector();
    }

}
#endif