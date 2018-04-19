using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _buttonControls : MonoBehaviour
{
    Vector3 lookdir;
    Vector3 pos;

    public Dictionary<string, List<GameObject>> inventory;

    private _gameSettings GM;
    private _audioController AM;
    private _elementMenu SelectedItem;
    private float waterLevel = 1f;
    private float fireLevel = 1f;
    private GameObject waterTool;
    private GameObject fireTool;
    private GameObject inventoryUI;
    private Rigidbody rb;
    private ParticleSystem waterPS;
    private ParticleSystem steamPS;
    private ParticleSystem firePS;
    private ParticleSystem smokePS;
    

    private Vector3 fireFinalPosition;
    private Quaternion fireFinalRotation;
    private Vector3 waterFinalPosition;
    private Quaternion waterFinalRotation;

    public string selectedItem { get { return SelectedItem.SelectedItem; } }
    // Use this for initialization
    void Awake()
    {
        //initialize inventory
        inventory = new Dictionary<string, List<GameObject>>();
        foreach (string item in _itemScript.getItemNames())
        {
            inventory.Add(item, new List<GameObject>());
        }
    }
    void Start()
    {
        GM = GameObject.Find("_EventSystem").GetComponent<_gameSettings>();
        AM = GameObject.Find("_EventSystem").GetComponent<_audioController>();

        waterTool = GameObject.Find("Water Arm");
        fireTool = GameObject.Find("Fire Arm");
        inventoryUI = GameObject.Find("Inventory");

        waterPS = GameObject.Find("Water").GetComponent<ParticleSystem>();
        steamPS = GameObject.Find("Steam").GetComponent<ParticleSystem>();
        firePS = GameObject.Find("Fire").GetComponent<ParticleSystem>();
        smokePS = GameObject.Find("Smoke").GetComponent<ParticleSystem>();

        SelectedItem = GameObject.Find("Selected Item").GetComponent<_elementMenu>();
        rb = this.GetComponent<Rigidbody>();

        fireFinalPosition = new Vector3(-.6f, 0, 0);
        waterFinalPosition = new Vector3(.6f, 0, 0);
        fireFinalRotation = Quaternion.Euler(new Vector3(-20,0,0));
        waterFinalRotation = Quaternion.Euler(new Vector3(-20,0,0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GM.isPaused || GM.isCrafting)
            return;

        fireLevel += (fireLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);
        waterLevel += (waterLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);

        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

        // use item
        if (Input.GetButtonUp("XButton") || Input.GetKeyDown(KeyCode.Q))
        {
            string selectedItem = SelectedItem.SelectedItem;
            if (selectedItem != "")
            {
                List<GameObject> items;
                inventory.TryGetValue(selectedItem, out items);
                if (items.Count > 0)
                {
                    GameObject firstItem = items[0];
                    if (firstItem.GetComponent<_itemScript>().Use(pos, lookdir))
                        items.Remove(firstItem); //only remove if it gets used

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("StartButton"))
        {
            GM.TogglePauseMenu();
        }

        // action button
        if (Input.GetKey(KeyCode.E) || Input.GetButtonDown("AButton"))
        {
            //Looking at an interactable object?
            RaycastHit hit;
            if (Physics.Raycast(pos, lookdir, out hit, 4f))
            {
                Transform other = hit.collider.transform;
                if (other.tag == "Inner Door")
                {
                    other.parent.SendMessage("tryOpen");
                }
                if (other.tag == "Pickup")
                {
                    List<GameObject> items;
                    inventory.TryGetValue(other.GetComponent<_itemScript>().item.ToString(), out items);
                    items.Add(other.gameObject.GetComponent<_itemScript>().pickup());
                    inventoryUI.SendMessage("updateInventory");
                }
                if (other.tag == "Crafting Table")
                {
                    GM.Craft(inventory);
                }
            }
        }

        // Tool section
        if ((Input.GetAxis("RightTrigger") > .2 || Input.GetMouseButton(1)) && (Input.GetAxis("LeftTrigger") > .2 || Input.GetMouseButton(0)))
        {
            toolAction(true);
        }
        else
        {
            toolAction(false);
        }


    }

    private void toolAction(bool both)
    {
        int fireDirection = both ? -1 : 1;

        if ((Input.GetAxis("RightTrigger") > .2 || Input.GetMouseButton(1)))
        {
            //raise Fire
            fireTool.transform.localRotation = Quaternion.Lerp(fireTool.transform.localRotation, fireFinalRotation, .1f);
        }
        else
        {
            //lower Fire
            fireTool.transform.localRotation = Quaternion.Lerp(fireTool.transform.localRotation, Quaternion.Euler(Vector3.zero), .05f);
        }

        if (Input.GetAxis("LeftTrigger") > .2 || Input.GetMouseButton(0))
        {
            //raise Water
            waterTool.transform.localRotation = Quaternion.Lerp(waterTool.transform.localRotation, waterFinalRotation, .1f);
        }
        else
        {
            //lower Water
            waterTool.transform.localRotation = Quaternion.Lerp(waterTool.transform.localRotation, Quaternion.Euler(Vector3.zero), .05f);
        }

        if (both)
        {
            fireTool.transform.localPosition = Vector3.Lerp(fireTool.transform.localPosition, fireFinalPosition, .1f);
            waterTool.transform.localPosition = Vector3.Lerp(waterTool.transform.localPosition, waterFinalPosition, .1f);
        }
        else
        {
            fireTool.transform.localPosition = Vector3.Lerp(fireTool.transform.localPosition, Vector3.zero, .1f);
            waterTool.transform.localPosition = Vector3.Lerp(waterTool.transform.localPosition, Vector3.zero, .1f);
        }

        // find if we should spray/fire or not
        float angle = 360 - waterTool.transform.localEulerAngles.x;
        bool spray = angle > 10 && angle < 25;
        angle = 360 - fireTool.transform.localEulerAngles.x;
        bool fire = angle > 10 && angle < 25;

        if (spray && waterLevel > 0 && (Input.GetAxis("LeftTrigger") > .8 || Input.GetMouseButton(0)))
        {
            //Water
            waterLevel -= Time.deltaTime / GM.toolUseTime;


            if (waterLevel > .01f && !waterPS.isEmitting)
            {
                AM.WaterAudio = true;
                waterPS.Play();
            }
        }
        else
        {
            waterPS.Stop();
            AM.WaterAudio = false;
        }


        if (both && fireLevel > 0 && waterLevel > 0)
        {
            if (!steamPS.isEmitting)
                steamPS.Play();
        }
        if (fire && fireLevel > 0 && (Input.GetAxis("RightTrigger") > .8 || Input.GetMouseButton(1)))
        {
            //Fire
            fireLevel -= Time.deltaTime / GM.toolUseTime;
            if (fireLevel > .01f && !firePS.isEmitting)
            {
                AM.FireAudio = true;
                firePS.Play();
                //smokePS.Play();
            }
        }
        else
        {
            firePS.Stop();
            AM.FireAudio = false;
            if (steamPS.isPlaying)
                steamPS.Stop();
        }

        //update UI on tool
        GameObject.Find("fire level").transform.localScale = new Vector3(fireLevel, 1, 1);
        GameObject.Find("fire percentage").GetComponent<Text>().text = ((int)(fireLevel * 100)).ToString() + "%";
        GameObject.Find("water level").transform.localScale = new Vector3(waterLevel, 1, 1);
        GameObject.Find("water percentage").GetComponent<Text>().text = ((int)(waterLevel * 100)).ToString() + "%";
    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.name == "Save Zone")
        {
            new WaitForSeconds(1f);
            GM.Save();
        }
    }
}
