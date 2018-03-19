using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class _itemScript : MonoBehaviour {

    public enum items { IRON, ALUMINUM, MERCURY, SILVER, MAGNESIUM, CESIUM, COPPER, SULPHUR, ZINC, THERMITE, BATTERY, COPPER_WIRE, LIQUID_NITROGEN};

    public items item;
    public GameObject explosion;
    public int delay = 20;

    private List<items> useable;
    private List<items> materials;
    private Material outlineGlow;

    public bool Useable { get { return useable.Contains(item); } }
    public bool Material { get { return materials.Contains(item); } }

    void Start()
    {
        useable = new List<items> { items.CESIUM, items.THERMITE, items.BATTERY, items.COPPER_WIRE, items.LIQUID_NITROGEN};
        materials = new List<items> { items.IRON, items.ALUMINUM, items.SILVER, items.MAGNESIUM, items.COPPER, items.SULPHUR, items.ZINC };
        //outlineGlow = this.gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (outlineGlow != null)
            outlineGlow.SetFloat("_OutlineWidth", Mathf.PingPong(Time.time/10, 0.05f)+1);
    }

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
            Debug.Log("Water");
            Water();
        }
    }

    public void Water()
    {
        switch (item)
        {
            case items.CESIUM:
                if (delay-- <= 0)
                {
                    GameObject temp = PrefabUtility.InstantiatePrefab(explosion) as GameObject;
                    temp.transform.position = transform.position;
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void Fire()
    {
        switch (item)
        {
            case items.THERMITE:
                break;
        }
    }

    public bool Use(Vector3 posOfUse, Vector3 lookDir)
    {
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
        }
        else if (Physics.Raycast(posOfUse, -Vector3.up, out hit, 4f))
        {
            placeLocation = hit.point;
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
