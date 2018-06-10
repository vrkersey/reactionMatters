using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SplineNodeHolder : MonoBehaviour {

    public float DistanceMod = 2;
    public float Dmod = 2;
    public SplineNode node;
    private void Update()
    {
        if(transform.parent!=null)
        {
            if (transform.hasChanged || transform.parent.hasChanged || DistanceMod != Dmod)
            {
                Debug.Log("moved");
                Dmod = DistanceMod;
                transform.hasChanged = false;
                transform.parent.hasChanged = false;
                Vector3 p2 = transform.position, p1 = transform.parent.position;
                var Len = Vector3.Distance(p1, p2);
                var dx = (p2.x - p1.x) / Len;
                var dy = (p2.y - p1.y) / Len;
                var dz = (p2.z - p1.z) / Len;
                Vector3 p3 = new Vector3(p1.x + Dmod * dx, p1.y + Dmod * dy, p1.z + Dmod * dz);
                node.SetPosition(transform.position);
                node.SetDirection(p3);
            }
        }
        else
        {
            if (transform.hasChanged || transform.parent.hasChanged || DistanceMod != Dmod)
            {
                Debug.Log("moved");
               
                transform.hasChanged = false;
                
                node.SetPosition(transform.position);
               
            }
        }
        
    }
}
