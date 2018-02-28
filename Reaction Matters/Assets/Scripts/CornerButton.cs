using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerButton : MonoBehaviour {
    public GameObject In;
    public GameObject Out;
    public GameObject selector;
    public GameObject Stay;
    public bool ShowInputs;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CreateIn()
    {
        DestroyImmediate(selector);
        DestroyImmediate(Out);
      //  In.SetActive(true);
        DestroyImmediate(gameObject.GetComponent<CornerButton>());
        transform.parent = Stay.transform;
    }
    public void CreateOut()
    {
        DestroyImmediate(selector);
        DestroyImmediate(In);
       // Out.SetActive(true);
        DestroyImmediate(gameObject.GetComponent<CornerButton>());
        transform.parent = Stay.transform;
    }
    public void DeleteSelf()
    {
        DestroyImmediate(selector);
        DestroyImmediate(In);
        DestroyImmediate(Out);
        DestroyImmediate(gameObject);
    }
}
