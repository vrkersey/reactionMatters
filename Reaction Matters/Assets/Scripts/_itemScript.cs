using System;
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
    private Dictionary<items, float> materialYs = new Dictionary<items, float>
    {
        {items.CESIUM, .2f},
        {items.THERMITE, 0f },
        {items.BATTERY, .161f },
        {items.COPPER_WIRE, .121f },
        {items.LIQUID_NITROGEN, 0f },
    };

    private List<Material> outlineGlow;
    private bool spawnItem = false;
    private float respawnTime;

    public bool Useable { get { return useable.Contains(item); } }
    public bool Material { get { return materials.Contains(item); } }

    void Start()
    {
        outlineGlow = new List<Material>();
        foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
            outlineGlow.Add(mr.material);

        respawnTime = GameObject.Find("_EventSystem").GetComponent<_gameSettings>().itemRespawnTimeInMinutes * 60;
    }

    void Update()
    {
        foreach (Material m in outlineGlow)
            m .SetFloat("_OutlineWidth", Mathf.PingPong(Time.time / 10, 0.05f) + 1);
    }
    public bool SpawnItem{ get{ return spawnItem; } set{spawnItem = value;} }

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
                    if (spawnItem)
                        StartCoroutine(respawn(gameObject, true, respawnTime));
                    else
                        Destroy(gameObject);
                    GameObject temp = Instantiate(explosion) as GameObject;
                    temp.transform.position = transform.position;
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
                    AudioSource audio = parentWall.Find("Audio Source").GetComponent<AudioSource>();
                    sparks.Play();
                    light.Play();
                    audio.PlayDelayed(.8f);
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
                            Destroy(gameObject, .5f);
                            return other.GetComponent<_doorController>().useItem(getName());
                        }
                        other = other.parent;
                    }
                }
                return drop(posOfUse, lookDir);
            case items.THERMITE:
                if (Physics.Raycast(posOfUse, lookDir, out hit, 5f) && hit.collider.tag == "Thermite Wall")
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

        if (gameObject.name == "Wall Thermite")
        {
            gameObject.SetActive(false);
            GameObject clone = Instantiate(gameObject);
            clone.name = "Thermite";
            clone.transform.localScale = new Vector3(0.1102068f, 0.1102068f, 0.1102068f);
            clone.SetActive(false);
            return clone;
        }

        if (spawnItem) {
            StartCoroutine(respawn(gameObject, true, respawnTime));
            GameObject clone = Instantiate(gameObject);
            clone.SetActive(false);
            return clone;
        }
        else
        {
            gameObject.SetActive(false);
            return gameObject;
        }
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
        float yplacement;
        materialYs.TryGetValue(item, out yplacement);
        placeLocation.y += yplacement;
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
