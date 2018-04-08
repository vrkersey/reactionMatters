using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _elementMenu : MonoBehaviour {
    
    public bool pauseElements;
    public Texture[] elements;

    private List<string> items;
    private Dictionary<string, Texture> elementsDict;
    private _buttonControls bc;

    private RawImage element;
    private Text text;
    private int index = 0;
    private bool locked;
    private string selectedItem = "";

    public string SelectedItem { get { return selectedItem; } }

	// Use this for initialization
	void Start () {

        element = transform.Find("Element").gameObject.GetComponent<RawImage>();
        element.texture = elements[index];
        elementsDict = new Dictionary<string, Texture>();
        foreach (Texture t in elements)
            elementsDict.Add(t.name, t);

        if (!pauseElements)
        {
            text = transform.Find("Text").gameObject.GetComponent<Text>();
        }
        items = new List<string>();
        bc = GameObject.Find("_Main Character").GetComponent<_buttonControls>();
	}
	
	// Update is called once per frame
	void Update () {
        if (pauseElements)
            PauseMenuAction();
        else
            SelectedElement();
	}

    private void SelectedElement()
    {
        string currentElement = element.texture.name;
        
        foreach (string s in bc.inventory.Keys)
        {
            List<GameObject> l;
            bc.inventory.TryGetValue(s, out l);
            if (l.Count > 0 && !items.Contains(s) && l[0].GetComponent<_itemScript>().Useable)
                items.Add(s);
            else if (l.Count == 0 && items.Contains(s))
                items.Remove(s);
        }

        index = items.IndexOf(currentElement);
        index = index < 0 ? 0 : index;

        if (items.Count == 0)
        {
            element.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            text.text = "0";
            selectedItem = "";
        }
        else
        {
            text.gameObject.SetActive(true);
            element.gameObject.SetActive(true);
            if (Input.GetButtonDown("RightBumper"))
            {
                index = (index + 1) % items.Count;
            }
            else if (Input.GetButtonDown("LeftBumper"))
            {
                index = index == 0 ? items.Count - 1 : index - 1;
            }
            Texture t;
            List<GameObject> l;
            elementsDict.TryGetValue(items[index], out t);
            bc.inventory.TryGetValue(items[index], out l);
            element.texture = t;
            text.text = l.Count.ToString();
            selectedItem = items[index].ToUpper();
        }      
    }

    private void PauseMenuAction()
    {
        if (Time.timeScale == 0f)
        {
            float input = Input.GetAxis("RightJoystickHorizontal");

            if (Math.Abs(input) > 0.15f)
            {
                IncrementIndex(input);
            }
            element.texture = elements[index];
        }
    }

    private void IncrementIndex(float input)
    {
        if (!locked)
        {
            if (input > 0)
            {
                index = (index + 1) % elements.Length;
            }
            else
            {
                index = index == 0 ? elements.Length - 1 : (index - 1);
            }
            StartCoroutine(Unlocker());
        }
    }

    private IEnumerator Unlocker()
    {
        locked = true;
        yield return new WaitForSecondsRealtime(.25f);
        locked = false;
    }

    public void switchIndex(string elementName)
    {
        for (int c = 0; c < elements.Length; c++)
        {
            if (elements[c].name.ToLower().Contains(elementName.ToLower()))
            {
                index = c;
                return;
            }
        }
    }
}
