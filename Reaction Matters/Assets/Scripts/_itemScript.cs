using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _itemScript : MonoBehaviour {

    public enum items { IRON, ALUMINUM, MERCURY, SILVER, MAGNESIUM, CESIUM, COPPER, SULPHUR, ZINC, THERMITE, BATTERY, COPPER_WIRE, LIQUID_NITROGEN};

    public items item;
    public GameObject explosion;

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Water"))
        {
            Water();
        }
        else if (c.gameObject.CompareTag("Fire"))
        {
            Fire();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.name == "Fire")
        {
            Fire();
        }
        else if (other.name == "Water")
        {
            Water();
        }
    }

    public void Water()
    {
        Debug.Log("you sprayed me with water");
        switch (item)
        {
            case items.CESIUM:
                Destroy(gameObject);
                explosion.SetActive(true);
                break;
        }
    }

    public void Fire()
    {
        Debug.Log("you lit it on fire");
        switch (item)
        {
            case items.THERMITE:
                break;
        }
    }

    public bool Use(Vector3 posOfUse, Vector3 lookDir)
    {
        Debug.Log("you used this item");
        switch (item)
        {
            case items.THERMITE:
                //place on wall/door
                break;
            default:
                return drop(posOfUse, lookDir);
        }

        return false;
    }

    private bool drop(Vector3 posOfUse, Vector3 lookDir)
    {
        lookDir.y = 0;
        RaycastHit hit;
        Vector3 placeLocation = Vector3.zero;

        if (Physics.Raycast(posOfUse, lookDir, out hit, 1.5f))
        {
            placeLocation = hit.point;
        }
        else
        {
            placeLocation = posOfUse + lookDir * 1.5f;
        }

        if (Physics.Raycast(placeLocation, -Vector3.up, out hit, 4f))
        {
            placeLocation = hit.point;
            placeLocation.y += this.GetComponent<SphereCollider>().radius;
        }
        else if (Physics.Raycast(posOfUse, -Vector3.up, out hit, 4f))
        {
            placeLocation = hit.point;
            placeLocation.y += this.GetComponent<SphereCollider>().radius;
        }
        else
        {
            return false;
        }

        transform.position = placeLocation;
        this.gameObject.SetActive(true);

        return true;
    }

    private RaycastHit doRayCast(Vector3 pos, Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, dir, out hit, 3f))
        {
            
            
        }
        return hit;
    }
    public static string[] getItemNames()
    {
        return Enum.GetNames(typeof(items));
    }
}
