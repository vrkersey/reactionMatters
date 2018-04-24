using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SwapMAt : MonoBehaviour {
	public Material ChangeTo;
	public string partialName = "Wall_Low:Wall_Low5:group1";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void swap()
	{
		foreach(GameObject g in Selection.gameObjects)
		{
			Debug.Log(g.transform.GetChild(0).Find(partialName).name);
			g.transform.GetChild(0).Find(partialName).GetComponent<Renderer>().material = ChangeTo;
		}
	}
}
#endif