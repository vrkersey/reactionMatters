using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _buttonControls : MonoBehaviour {

    Vector3 lookdir;
    Vector3 pos;
    int items;

    public int pickups { get { return items; } }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        bool interact = false;

        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

        if (Input.GetKey(KeyCode.E) || Input.GetButton("AButton"))
        {
            interact = true;
        }


        //Looking at an interactable object?
        RaycastHit hit;
        if (Physics.Raycast(pos, lookdir, out hit, 3f))
        {
            GameObject other = hit.collider.gameObject;
            if (other.tag == "Door" && interact)
            {
                Debug.Log("opened door");
                StartCoroutine(openDoor(other));
            }
            if (other.tag == "Pickup" && interact)
            {
                Debug.Log("picked up object");
                other.SetActive(false);
                items++;
            }
        }
	}

    IEnumerator openDoor(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(5f);
        obj.SetActive(true);
    }
}
