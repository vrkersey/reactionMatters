using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _wallController : MonoBehaviour {

    public bool galiumWall = true;
    public float meltTime = 5f;

    private float heat = 0;
    private float tempDispersion = .025f;

	// Use this for initialization
	void Start () {
        meltTime *= 5;
	}
	
	// Update is called once per frame
	void Update () {
        heat = heat - tempDispersion > 0 ? heat - tempDispersion : heat;
        Debug.Log(heat);
        if (meltTime < heat)
            this.gameObject.SetActive(false);
	}

    void OnParticleCollision(GameObject other)
    {
        if (other.name == "Steam")
        {
            heat++;
        }
    }
}
