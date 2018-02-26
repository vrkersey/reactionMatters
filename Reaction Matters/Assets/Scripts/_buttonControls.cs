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
    private float waterLevel = 1f;
    private float fireLevel = 1f;
    private GameObject waterTool;
    private GameObject fireTool;
    private Rigidbody rb;
    private bool grounded;
    private GameObject previousWaterBullet;
    private GameObject previousFireBullet;

    public bool Grounded { get { return grounded; } }

    // Use this for initialization
    void Start () {
        GM = GameObject.Find("_EventSystem").GetComponent<_gameSettings>();
        waterTool = GameObject.Find("Water Arm");
        fireTool = GameObject.Find("Fire Arm");

        rb = this.GetComponent<Rigidbody>();

        //initialize inventory
        inventory = new Dictionary<string, List<GameObject> >();
        foreach (string item in _itemScript.getItemNames())
        {
            inventory.Add(item, new List<GameObject>());
        }

    }

    // Update is called once per frame
    void Update () {
        fireLevel += (fireLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);
        waterLevel += (waterLevel <= 1 ? Time.deltaTime / GM.toolRespawnTime : 0);

        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

        // jump
        if (grounded && (Input.GetKey(KeyCode.Space) || Input.GetButtonDown("AButton")))
        {
            Vector3 upDir = new Vector3(0, 1, 0);
            rb.AddForce(upDir * GM.jumpHeight, ForceMode.VelocityChange);
            grounded = false;
        }

        if (Input.GetKey(KeyCode.E) || Input.GetButtonDown("BButton"))
        {
            //Looking at an interactable object?
            RaycastHit hit;
            if (Physics.Raycast(pos, lookdir, out hit, 3f))
            {
                GameObject other = hit.collider.gameObject;
                Debug.Log(other.name);
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

        if (Input.GetAxis("RightTrigger") > .2 || Input.GetMouseButton(1))
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

        if (fire && fireLevel > 0 && (Input.GetAxis("RightTrigger") > .8 || Input.GetMouseButton(1)))
        {
            //Fire
            fireLevel -= Time.deltaTime / GM.toolUseTime;
            if (fireLevel > .01f)
                useTool("Fire");
        }
        if (spray && waterLevel > 0 && (Input.GetAxis("LeftTrigger") > .8 || Input.GetMouseButton(0)))
        {
            //Water
            waterLevel -= Time.deltaTime / GM.toolUseTime;
            if (waterLevel > .01f)
                useTool("Water");
            //https://forum.unity.com/threads/water-gun-water-stream.194098/
        }

        //update UI on tool
        GameObject.Find("fire level").transform.localScale = new Vector3(fireLevel, 1, 1);
        GameObject.Find("fire percentage").GetComponent<Text>().text = ((int)(fireLevel * 100)).ToString() + "%";
        GameObject.Find("water level").transform.localScale = new Vector3(waterLevel, 1, 1);
        GameObject.Find("water percentage").GetComponent<Text>().text = ((int)(waterLevel * 100)).ToString() + "%";
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

    void useTool(string tool){
        GameObject bullet = null;
        float time;
        if (tool == "Water")
        {
            bullet = Instantiate(bulletPrefab, waterStartPosition.position, waterStartPosition.rotation);
            time = 2.0f;
            if (previousWaterBullet != null && Vector3.Distance(previousWaterBullet.transform.position, bullet.transform.position) < .5f)
                previousWaterBullet.GetComponent<_bullet>().DrawLineTo(bullet);
            previousWaterBullet = bullet;
        }
        else
        {
            bullet = Instantiate(bulletPrefab, fireStartPosition.position, fireStartPosition.rotation);
            time = .07f;
            if (previousFireBullet != null && Vector3.Distance(previousFireBullet.transform.position, bullet.transform.position) < .5f)
                previousFireBullet.GetComponent<_bullet>().DrawLineTo(bullet);
            previousFireBullet = bullet;
        }

        bullet.tag = tool;
        bullet.GetComponent<Rigidbody>().AddForce(lookdir * 5f);
        Destroy(bullet, time);
    }
}
