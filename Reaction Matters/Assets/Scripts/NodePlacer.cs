using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePlacer : MonoBehaviour {
    public Vector3 Offset = Vector3.zero;
    public GameObject Snap1, Snap2, Snap3, Snap4;
    private void OnValidate()
    {
        if(Snap1!=null)
        {
            Snap1.transform.parent = transform;
            Snap1.transform.localPosition = Vector3.forward + Offset;
        }
        if (Snap2 != null)
        {
            Snap2.transform.parent = transform;
            Snap2.transform.localPosition = Vector3.left + Offset;
        }
        if (Snap3 != null)
        {
            Snap3.transform.parent = transform;
            Snap3.transform.localPosition = Vector3.right + Offset;
        }
        if (Snap4 != null)
        {
            Snap4.transform.parent = transform;
            Snap4.transform.localPosition = Vector3.back + Offset;
        }
           
       

       
       
      
        

    }
}
