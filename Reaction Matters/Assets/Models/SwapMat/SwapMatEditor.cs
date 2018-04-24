using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SwapMAt))]
public class SwapMatEditor : Editor {


	// Update is called once per frame
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		SwapMAt build = (SwapMAt)target;
		if (GUILayout.Button("Swap"))
		{
			build.swap();
		}
	}
}
#endif
