using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _buttonControls : MonoBehaviour {

    Vector3 lookdir;
    Vector3 pos;

    public Dictionary<string, List<GameObject> > inventory;

    private _gameSettings GM;

    // Use this for initialization
    void Start () {
        GM = GameObject.Find("Level Settings").GetComponent<_gameSettings>();

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


        lookdir = this.transform.Find("Mover").transform.forward;
        pos = this.transform.Find("Mover").transform.position;

        if (Input.GetKey(KeyCode.E) || Input.GetButton("AButton"))
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
                    Debug.Log("picked up " + other.GetComponent<_itemScript>().item.ToString());
                    List<GameObject> items;
                    inventory.TryGetValue(other.GetComponent<_itemScript>().item.ToString(), out items);
                    items.Add(other);
                    Debug.Log("I have " + items.Count + " of them");
                    other.SetActive(false);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButton("AButton"))
        {
            GM.TogglePauseMenu();
        }


        
	}

    IEnumerator openDoor(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(5f);
        obj.SetActive(true);
    }
}
