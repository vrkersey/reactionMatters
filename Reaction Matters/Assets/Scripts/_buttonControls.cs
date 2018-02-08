using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _buttonControls : MonoBehaviour {

    Vector3 lookdir;
    Vector3 pos;

    public Dictionary<string, List<GameObject> > inventory;
 
    // Use this for initialization
    void Start () {
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
                StartCoroutine(openDoor(other));
            }
            if (other.tag == "Pickup" && interact)
            {
                Debug.Log("picked up " + other.GetComponent<_itemScript>().item.ToString());
                List<GameObject> items;
                inventory.TryGetValue(other.GetComponent<_itemScript>().item.ToString(), out items);
                items.Add(other);
                Debug.Log("I have " + items.Count + " of them");
                other.SetActive(false);
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
