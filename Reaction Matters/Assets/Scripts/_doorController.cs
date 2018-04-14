using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _doorController : MonoBehaviour {

    public bool batteryDoor = false;
    public bool copperWireDoor = false;
    public Texture lockedImage;
    public Texture unlockedImage;

    private bool locked = false;
    private RawImage lockedIcon1;
    private RawImage lockedIcon2;

    public bool Locked { get { return locked; } }

	// Use this for initialization
	void Start () {
        if (batteryDoor || copperWireDoor)
            locked = true;
        lockedIcon1 = transform.Find("Canvas").Find("lockedImage").GetComponent<RawImage>();
        lockedIcon2 = transform.Find("Canvas (1)").Find("lockedImage (1)").GetComponent<RawImage>();
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
            lockedIcon1.texture = unlockedImage;
            lockedIcon2.texture = unlockedImage;
            locked = false;
            ret = true;
        }
        if (copperWireDoor && itemName == "COPPER_WIRE")
        {
            lockedIcon1.texture = unlockedImage;
            lockedIcon2.texture = unlockedImage;
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
