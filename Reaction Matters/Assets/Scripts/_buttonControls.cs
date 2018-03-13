using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _buttonControls : MonoBehaviour {

    Vector3 lookdir;
    Vector3 pos;

    public Dictionary<string, List<GameObject> > inventory;
    public GameObject bulletPrefab;
    public Transform waterStartPosition;
    public Transform fireStartPosition;

    private _gameSettings GM;
    private _elementMenu SelectedItem;
    private float waterLevel = 1f;
    private float fireLevel = 1f;
    private GameObject waterTool;
    private GameObject fireTool;
    private Rigidbody rb;
    private bool grounded;
    private GameObject previousWaterBullet;
    private GameObject previousFireBullet;
    private ParticleSystem waterPS;
    private ParticleSystem steamPS;
    private ParticleSystem firePS;
    private ParticleSystem smokePS;
    private AudioSource waterAudio;
    private AudioSource fireAudio;

    public bool Grounded { get { return grounded; } }

    // Use this for initialization
    void Start () {
        GM = GameObject.Find("_EventSystem").GetComponent<_gameSettings>();
        waterTool = GameObject.Find("Water Arm");
        fireTool = GameObject.Find("Fire Arm");

        waterPS = GameObject.Find("Water").GetComponent<ParticleSystem>();
        steamPS = GameObject.Find("Steam").GetComponent<ParticleSystem>();
        firePS = GameObject.Find("Fire").GetComponent<ParticleSystem>();
        smokePS = GameObject.Find("Smoke").GetComponent<ParticleSystem>();

        waterAudio = GameObject.Find("Water Audio").GetComponent<AudioSource>();
        fireAudio = GameObject.Find("Fire Audio").GetComponent<AudioSource>();

        SelectedItem = GameObject.Find("Selected Item").GetComponent<_elementMenu>();
        rb = this.GetComponent<Rigidbody>();

        //initialize inventory
        inventory = new Dictionary<string, List<GameObject> >();
        foreach (string item in _itemScript.getItemNames())
        {
            inventory.Add(item, new List<GameObject>());
        }

    }

    // Update is called once per frame
    void FixedUpdate () {
        fireLevel += (fireLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);
        waterLevel += (waterLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);

        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

        //// jump
        //if (grounded && (Input.GetKey(KeyCode.Space) || Input.GetButtonDown("AButton")))
        //{
        //    Vector3 upDir = new Vector3(0, 1, 0);
        //    rb.AddForce(upDir * GM.jumpHeight, ForceMode.VelocityChange);
        //    grounded = false;
        //}

        // use item
        if (Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.Q))
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

        // action button
        if (Input.GetKey(KeyCode.E) || Input.GetButtonDown("AButton"))
        {
            //Looking at an interactable object?
            RaycastHit hit;
            if (Physics.Raycast(pos, lookdir, out hit, 3f))
            {
                GameObject other = hit.collider.gameObject;

                if (other.tag == "Door")
                {
                    StartCoroutine(openDoor(other));
                }
                if (other.tag == "Pickup")
                {
                    List<GameObject> items;
                    inventory.TryGetValue(other.GetComponent<_itemScript>().item.ToString(), out items);
                    items.Add(other);
                    other.SetActive(false);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("StartButton"))
        {
            GM.TogglePauseMenu();
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

    IEnumerator openDoor(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(5f);
        obj.SetActive(true);
    }

    private void rotateTool(Transform arm, float toAngle, int speed)
    {
        Vector3 direction = new Vector3(toAngle, arm.transform.localEulerAngles.y, arm.transform.localEulerAngles.z);
        Quaternion targetRotation = Quaternion.Euler(direction);
        arm.transform.localRotation = Quaternion.Lerp(arm.transform.localRotation, targetRotation, Time.deltaTime * speed);
    }

    public float getWaterLevel()
    {
        return waterLevel;
    }
    public float getFireLevel()
    {
        return fireLevel;
    }

    void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("Floor"))
        {
            grounded = true;
        }
    }

    private void toolAction(bool both)
    {
        int fireDirection = both ? -1 : 1;

        if ((Input.GetAxis("RightTrigger") > .2 || Input.GetMouseButton(1)) && !both)
        {
            //raise Fire
            rotateTool(fireTool.transform, -20, 5);
        }
        else
        {
            //lower Fire
            rotateTool(fireTool.transform, 0, 3);
        }

        if (Input.GetAxis("LeftTrigger") > .2 || Input.GetMouseButton(0))
        {
            //raise Water
            rotateTool(waterTool.transform, -20, 5);
        }
        else
        {
            //lower Water
            rotateTool(waterTool.transform, 0, 3);
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
                waterAudio.Play();
                waterPS.Play();
            }
        }
        else
        {
            waterPS.Stop();
            waterAudio.Stop();
        }


        if (both)
        {
            fireLevel -= Time.deltaTime / GM.toolUseTime;
            firePS.Stop();
            if (!steamPS.isEmitting)
                steamPS.Play();
            if (!fireAudio.isPlaying)
                fireAudio.Play();
        }
        else if (fire && fireLevel > 0 && (Input.GetAxis("RightTrigger") > .8 || Input.GetMouseButton(1)))
        {
            //Fire
            fireLevel -= Time.deltaTime / GM.toolUseTime;
            if (fireLevel > .01f && !firePS.isEmitting)
            {
                fireAudio.Play();
                firePS.Play();
                //smokePS.Play();
            }
        }
        else
        {
            firePS.Stop();
            fireAudio.Stop();
            if (steamPS.isPlaying)
                steamPS.Stop();
        }

        //update UI on tool
        GameObject.Find("fire level").transform.localScale = new Vector3(fireLevel, 1, 1);
        GameObject.Find("fire percentage").GetComponent<Text>().text = ((int)(fireLevel * 100)).ToString() + "%";
        GameObject.Find("water level").transform.localScale = new Vector3(waterLevel, 1, 1);
        GameObject.Find("water percentage").GetComponent<Text>().text = ((int)(waterLevel * 100)).ToString() + "%";
    }
}
