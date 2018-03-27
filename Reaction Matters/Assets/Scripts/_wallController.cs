using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _wallController : MonoBehaviour {

    public bool galiumWall = true;
    public float meltTime = 5f;
    public bool redShiftWall = true;

    private float heat = 0;
    private float tempDispersion = .25f;
    private float heating;
    private Material material;
    private Color startColor;
    private Color changingColor;

    private float redSpeed;

    private float red;
    private float blue;
    private float green;
    private float alpha;

    // Use this for initialization
    void Start () {
        meltTime *= 45;
        heating = -.5f;
        material = GetComponent<MeshRenderer>().material;
        startColor = material.color;
        changingColor = new Color();
        changingColor.r = startColor.r;
        changingColor.g = startColor.g;
        changingColor.b = startColor.b;
        changingColor.a = startColor.a;

        redSpeed = redShiftWall ? (1.2f - startColor.r) / meltTime : 0;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Time.time - heating < .5f)
        {
            changingColor.r = changingColor.r + redSpeed <= 1 ? changingColor.r + redSpeed : 1;
            heat++;
        }
        else
        {
            changingColor.r = changingColor.r >= startColor.r ? changingColor.r - (redSpeed * tempDispersion) : startColor.r;
            heat = heat - tempDispersion > 0 ? heat - tempDispersion : 0;
        }
        if (material != null)
            material.color = changingColor;
        if (meltTime < heat)
            this.gameObject.SetActive(false);
	}

    void OnParticleCollision(GameObject other)
    {
        if (other.name == "Steam")
        {
            heating = Time.time;
        }
    }
}
