using UnityEngine;
using System.Collections;
 
public class PhysicalExplosion : MonoBehaviour 
{
    public float Radius;// explosion radius
    public float Force;// explosion forse
    void Update () 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);// create explosion
        for(int i=0; i<hitColliders.Length; i++)
        {              
            if(hitColliders[i].CompareTag("CanDestroy"))// if tag CanBeRigidbody
            {
                if(!hitColliders[i].GetComponent<Rigidbody>())
                {
                hitColliders[i].gameObject.AddComponent<Rigidbody>();
                }
                hitColliders[i].GetComponent<Rigidbody>().AddExplosionForce(Force, transform.position, Radius, 0.0F); // push game object
                Destroy(hitColliders[i].GetComponent<Rigidbody>(), 15f);
                Destroy(hitColliders[i].GetComponent<MeshCollider>(), 15f);
                if (hitColliders[i].name.Contains("polySurface2") || hitColliders[i].name.Contains("polySurface5"))
                {
                    Destroy(hitColliders[i].gameObject, 5f);
                }
            }
			
        }
        Destroy(gameObject,1f);// destroy explosion
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,Radius);
    }
}