using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _bullet : MonoBehaviour {

    private GameObject otherBullet;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start () {

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.startWidth = .02f;
        lineRenderer.endWidth = .02f;
    }
	
	// Update is called once per frame
	void Update () {
        lineRenderer.SetPosition(0, this.transform.position);

        if (otherBullet != null && Vector3.Distance(transform.position, otherBullet.transform.position) < .25f )
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
