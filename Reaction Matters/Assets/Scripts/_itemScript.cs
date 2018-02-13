using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _itemScript : MonoBehaviour {

    public enum items { IRON, ALUMINUM, MERCURY, SILVER, MAGNESIUM, CESIUM, COPPER, SULPHUR, ZINC };

    public items item;

    public void Water()
    {
        Debug.Log("you sprayed me with water");
        switch (item)
        {
            case items.IRON:
                break;
            case items.ALUMINUM:
                break;
            case items.MERCURY:
                break;
            case items.SILVER:
                break;
            case items.MAGNESIUM:
                break;
            case items.CESIUM:
                break;
            case items.COPPER:
                break;
            case items.SULPHUR:
                break;
            case items.ZINC:
                break;
        }
    }

    public void Fire()
    {
        Debug.Log("you lit it on fire");
        switch (item)
        {
            case items.IRON:
                break;
            case items.ALUMINUM:
                break;
            case items.MERCURY:
                break;
            case items.SILVER:
                break;
            case items.MAGNESIUM:
                break;
            case items.CESIUM:
                break;
            case items.COPPER:
                break;
            case items.SULPHUR:
                break;
            case items.ZINC:
                break;
        }
    }

    public string[] getItemNames()
    {
        return Enum.GetNames(typeof(items));
    }
}
