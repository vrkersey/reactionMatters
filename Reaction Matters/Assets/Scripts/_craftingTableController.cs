using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _craftingTableController : MonoBehaviour {

    private float cooldown;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cooldown = cooldown <= 0 ? 0 : cooldown - Time.deltaTime;
	}

    public bool tryToRefill()
    {
        if (cooldown == 0)
        {
            cooldown = 360;
            return true;
        }
        return false;
    }
}
