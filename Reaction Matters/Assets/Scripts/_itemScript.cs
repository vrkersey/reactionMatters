﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _itemScript : MonoBehaviour {

    public enum items { IRON, ALUMINUM, MERCURY, SILVER, MAGNESIUM, CESIUM, COPPER, LITHIUM, MANGANESE, THERMITE, BATTERY, COPPER_WIRE, LIQUID_NITROGEN};

    public items item;
    public GameObject explosion;
    public int delay = 20;

    private List<items> useable = new List<items> { items.CESIUM, items.THERMITE, items.BATTERY, items.COPPER_WIRE, items.LIQUID_NITROGEN };
    private List<items> materials = new List<items> { items.IRON, items.ALUMINUM, items.SILVER, items.MAGNESIUM, items.COPPER, items.LITHIUM, items.MANGANESE };
    private Material outlineGlow;
    private bool spawnItem = false;

    public bool Useable { get { return useable.Contains(item); } }
    public bool Material { get { return materials.Contains(item); } }

    void Start()
    {
        if (gameObject.GetComponent<MeshRenderer>() != null)
            outlineGlow = gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (outlineGlow != null)
            outlineGlow.SetFloat("_OutlineWidth", Mathf.PingPong(Time.time/10, 0.05f)+1);
    }
    public bool SpawnItem{ get{ return spawnItem; } set{spawnItem = value;} }

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
        switch (item)
        {
            case items.CESIUM:
                if (delay-- <= 0)
                {
                    GameObject temp = Instantiate(explosion) as GameObject;
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
                if (this.name == "Wall Thermite")
                {
                    Transform parentWall = this.transform.parent.parent.parent;
                    GameObject solidWall = parentWall.Find("Solid Wall").gameObject;
                    GameObject holedWall = parentWall.Find("Holed Wall").gameObject;
                    ParticleSystem sparks = parentWall.Find("Sparks").GetComponent<ParticleSystem>();
                    ParticleSystem light = parentWall.Find("Light").GetComponent<ParticleSystem>();
                    sparks.Play();
                    light.Play();
                    StartCoroutine(respawn(holedWall, true, 3));
                    StartCoroutine(respawn(solidWall, false, 3));
                }
                else
                {
                    //regular thermite
                }
                break;
        }
    }

    public bool Use(Vector3 posOfUse, Vector3 lookDir)
    {
        RaycastHit hit;
        switch (item)
        {
            case items.BATTERY:
            case items.COPPER_WIRE:
                
                if (Physics.Raycast(posOfUse, lookDir, out hit, 3f))
                {
                    Transform other = hit.collider.transform;
                    bool keepLooking = true;
                    while (other != null && keepLooking)
                    {
                        if (other.tag == "Door")
                        {
                            return other.GetComponent<_doorController>().useItem(getName());
                        }
                        other = other.parent;
                    }
                }
                return drop(posOfUse, lookDir);
            case items.THERMITE:
                if (Physics.Raycast(posOfUse, lookDir, out hit, 3f) && hit.collider.tag == "Thermite Wall")
                {
                    hit.collider.transform.Find("Wall Thermite").gameObject.SetActive(true);
                    Destroy(gameObject, .5f);
                    return true;
                }
                else
                {
                    return drop(posOfUse, lookDir);
                }
                
            default:
                return drop(posOfUse, lookDir);
        }
    }

    IEnumerator respawn(GameObject obj, bool isActive, float time)
    {
        obj.SetActive(!isActive);
        yield return new WaitForSeconds(time);
        obj.SetActive(isActive);
    }

    public GameObject pickup(){
        GameObject clone = Instantiate(gameObject);
        clone.SetActive(false);
        
        return clone;
    }

    private bool drop(Vector3 posOfUse, Vector3 lookDir)
    {
        lookDir.y = 0;
        RaycastHit hit;
        Vector3 placeLocation = Vector3.zero;
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        Bounds bounds = mesh != null ? mesh.bounds : new Bounds();

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

        placeLocation.y += bounds.max.y/2;
        transform.position = placeLocation;
        this.gameObject.SetActive(true);

        return true;
    }

    public string getName()
    {
        return Enum.GetName(typeof(items), item);
    }

    public static string[] getItemNames()
    {
        return Enum.GetNames(typeof(items));
    }
}
