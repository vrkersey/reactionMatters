using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _buttonControls : MonoBehaviour {

    Vector3 lookdir;
    Vector3 pos;

    public Dictionary<string, List<GameObject> > inventory;


    private _gameSettings GM;
    private float waterLevel = 1f;
    private float fireLevel = 1f;
    private GameObject waterTool;
    private GameObject fireTool;

    // Use this for initialization
    void Start () {
        GM = GameObject.Find("EventSystem").GetComponent<_gameSettings>();
        waterTool = GameObject.Find("Water Arm");
        fireTool = GameObject.Find("Fire Arm");

        //initialize inventory
        inventory = new Dictionary<string, List<GameObject> >();
        GameObject pickup = GameObject.FindGameObjectWithTag("Pickup");
        foreach (string item in pickup.GetComponent<_itemScript>().getItemNames())
        {
            inventory.Add(item, new List<GameObject>());
        }
	}
	
	// Update is called once per frame
	void Update () {
        fireLevel += (fireLevel <= 100 ? Time.deltaTime / 100 : 0);
        waterLevel += (waterLevel <= 100 ? Time.deltaTime / 100 : 0);

        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

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

        if (Input.GetAxis("RightTrigger") > .8)
        {
            //Fire
            fireLevel -= Time.deltaTime/20;
        }
        if (Input.GetAxis("LeftTrigger") > .8)
        {
            //Water
            waterLevel -= Time.deltaTime/20;
        }


        if (Input.GetAxis("RightTrigger") > .2)
        {
            //raise Fire
            rotateTool(fireTool.transform, -20, 5);
        }
        else
        {
            //lower Fire
            rotateTool(fireTool.transform, 0, 3);
        }

        if (Input.GetAxis("LeftTrigger") > .2)
        {
            //raise Water
            rotateTool(waterTool.transform, -20, 5);
        }
        else
        {
            //lower Water
            rotateTool(waterTool.transform, 0, 3);
        }


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
        Vector3 direction = new Vector3(arm.rotation.eulerAngles.x + toAngle, arm.rotation.eulerAngles.y, arm.rotation.eulerAngles.z);
        Quaternion targetRotation = Quaternion.Euler(direction);
        arm.rotation = Quaternion.Lerp(arm.rotation, targetRotation, Time.deltaTime * speed);
    }

    public float getWaterLevel()
    {
        return waterLevel;
    }
    public float getFireLevel()
    {
        return fireLevel;
    }
}
