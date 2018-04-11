using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _doorController : MonoBehaviour {

    public bool batteryDoor = false;
    public bool copperWireDoor = false;

    private bool locked = false;

	// Use this for initialization
	void Start () {
        if (batteryDoor || copperWireDoor)
            locked = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool tryOpen()
    {
        if (locked)
            return false;
        StartCoroutine(openDoor(transform.Find("DoorIn:polySurface1").gameObject));
        return true;
    }

    public bool useItem(string itemName)
    {
        bool ret = false;
        if (batteryDoor && itemName == "BATTERY")
        {
            locked = false;
            ret = true;
        }
        if (copperWireDoor && itemName == "COPPER_WIRE")
        {
            locked = false;
            ret = true;
        }

        return ret;
    }

    IEnumerator openDoor(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(5f);
        obj.SetActive(true);
    }
}
