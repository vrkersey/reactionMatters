using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _itemScript : MonoBehaviour {

    public enum items { IRON, ALUMINUM, MERCURY, SILVER, MAGNESIUM, CESIUM, COPPER, SULPHUR, ZINC };

    public items item;

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

    public void Water()
    {
        Debug.Log("you sprayed me with water");
        switch (item)
        {
            case items.IRON:
                break;
            case items.ALUMINUM:
                break;
            case items.MERCURY:
                break;
            case items.SILVER:
                break;
            case items.MAGNESIUM:
                break;
            case items.CESIUM:
                Debug.Log("EXPLOSION!!!");
                break;
            case items.COPPER:
                break;
            case items.SULPHUR:
                break;
            case items.ZINC:
                break;
        }
    }

    public void Fire()
    {
        Debug.Log("you lit it on fire");
        switch (item)
        {
            case items.IRON:
                break;
            case items.ALUMINUM:
                break;
            case items.MERCURY:
                break;
            case items.SILVER:
                break;
            case items.MAGNESIUM:
                break;
            case items.CESIUM:
                break;
            case items.COPPER:
                break;
            case items.SULPHUR:
                break;
            case items.ZINC:
                break;
        }
    }

    public void Use(Vector3 posOfUse, Vector3 lookDir)
    {
        Debug.Log("you used this item");
        switch (item)
        {
            case items.IRON:
                break;
            case items.ALUMINUM:
                break;
            case items.MERCURY:
                break;
            case items.SILVER:
                break;
            case items.MAGNESIUM:
                break;
            case items.CESIUM:
                break;
            case items.COPPER:
                break;
            case items.SULPHUR:
                break;
            case items.ZINC:
                break;
        }
        drop(posOfUse, lookDir); 
    }

    private void drop(Vector3 posOfUse, Vector3 lookDir)
    {
        lookDir.y = 0;

        Vector3 placeLocation = posOfUse + lookDir * 1.5f;

        RaycastHit hit;
        if (Physics.Raycast(placeLocation, -Vector3.up, out hit, 3f))
        {
            placeLocation = hit.point;
            placeLocation.y += this.GetComponent<SphereCollider>().radius;
        }
        else if (Physics.Raycast(posOfUse, -Vector3.up, out hit, 3f))
        {
            placeLocation = hit.point;
            placeLocation.y += this.GetComponent<SphereCollider>().radius;
        }

        transform.position = placeLocation;
        this.gameObject.SetActive(true);
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
