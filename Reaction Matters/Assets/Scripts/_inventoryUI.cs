using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _inventoryUI : MonoBehaviour {
    private Text aluminum;
    private Text copper;
    private Text iron;
    private Text lithium;
    private Text manganese;
    private _buttonControls bc;

    // Use this for initialization
    void Start () {
        aluminum = transform.Find("Aluminum").GetComponentInChildren<Text>();
        copper = transform.Find("Copper").GetComponentInChildren<Text>(); ;
        iron = transform.Find("Iron").GetComponentInChildren<Text>(); ;
        lithium = transform.Find("Lithium").GetComponentInChildren<Text>(); ;
        manganese = transform.Find("Manganese").GetComponentInChildren<Text>(); ;
        bc = GameObject.Find("_Main Character").GetComponent<_buttonControls>();
    }
	
    public void updateInventory()
    {
        List<GameObject> l;
        bc.inventory.TryGetValue("ALUMINUM", out l);
        aluminum.text = l.Count.ToString();

        bc.inventory.TryGetValue("COPPER", out l);
        copper.text = l.Count.ToString();

        bc.inventory.TryGetValue("IRON", out l);
        iron.text = l.Count.ToString();

        bc.inventory.TryGetValue("LITHIUM", out l);
        lithium.text = l.Count.ToString();

        bc.inventory.TryGetValue("MANGANESE", out l);
        manganese.text = l.Count.ToString();
    }
}
