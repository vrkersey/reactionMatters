using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _bullet : MonoBehaviour {

    public Material waterMaterial;
    public Material fireMaterial;

    private GameObject otherBullet;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private Material material;
    private bool water;

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody>();
        if (this.tag == "Water")
        {
            material = waterMaterial;
            water = true;
        }
        else if (this.tag == "Fire")
        {
            material = fireMaterial;
            rb.useGravity = false;
            water = false;
        }
        else
        {
            material = new Material(Shader.Find("Particles/Additive"));
        }
        this.GetComponent<MeshRenderer>().material = material;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        lineRenderer.startWidth = .02f;
        lineRenderer.endWidth = .02f;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        lineRenderer.SetPosition(0, this.transform.position);

        if (otherBullet != null && Vector3.Distance(transform.position, otherBullet.transform.position) < .35f )
        {
            //update line
            float dist = Vector3.Distance(transform.position, otherBullet.transform.position);
           
            Vector3 line = dist * Vector3.Normalize(otherBullet.transform.position - this.transform.position) + this.transform.position;
            
            lineRenderer.SetPosition(1, line);
          
        }
        else
        {
            lineRenderer.SetPosition(1, this.transform.position);
        }
    }

    public void DrawLineTo(GameObject other)
    {
        otherBullet = other;
    }
}
