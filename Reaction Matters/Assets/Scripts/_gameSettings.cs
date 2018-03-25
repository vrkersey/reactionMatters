﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _gameSettings : MonoBehaviour {

    [Space(10)]
    [Header("Level things")]
    public GameObject pauseMenu;
    public GameObject craftingMenu;

    public float startTimeInMinutes = 5f;
    [Tooltip("Time in Seconds to Fill Tool from 0 to 100")]
    public float toolRespawnTime = 100f;
    [Tooltip("Time in Seconds to Use Tool from 100 to 0")]
    public float toolUseTime = 20f;
    [Space(10)]
    [Header("Player things")]
    public float movementSpeed = 15f;
    public float sensitivity = 15f;
    public float jumpHeight = 2.5f;
    

    private float timeRemaining;
    private Text timeDisplay;
    private _movementControls MC;
    private _elementMenu EM;
    private _buttonControls BM;

    private Transform resumeButton;
    private Transform quitButton;
    private bool paused = false;
    private bool crafting = false;
    private List<string> itemsDiscovered = new List<string>();
    private Transform craftingSelected;

    // Use this for initialization
    void Start () {
        timeRemaining = startTimeInMinutes * 60;
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();

        MC = GameObject.Find("_Main Character").GetComponentInChildren<_movementControls>();
        BM = GameObject.Find("_Main Character").GetComponent<_buttonControls>();
        EM = pauseMenu.transform.Find("Elements").GetComponentInChildren<_elementMenu>();

        resumeButton = pauseMenu.transform.Find("Menu").Find("Resume");
        quitButton = pauseMenu.transform.Find("Menu").Find("Quit");
    }
	
	// Update is called once per frame
	void Update () {
        float guiTime = timeRemaining > 0 ? timeRemaining - Time.fixedTime : 0;
        int minutes = (int)guiTime / 60;
        int seconds = (int)guiTime % 60;

        string textTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        timeDisplay.text = textTime;

        if (guiTime == 0)
        {
            Reset();
        }

        if (paused)
        {
            if (Input.GetButtonDown("BButton"))
                TogglePauseMenu();
        }
        if (crafting)
        {
            if (Input.GetButtonDown("BButton"))
            {
                ToggleCraftingMenu();
            }
        }
    }

    public void Craft(Dictionary<string, List<GameObject>> inventory)
    {
        setupCraftingUI(inventory);
        ToggleCraftingMenu();
    }

    private void ToggleCraftingMenu()
    {
        if (craftingMenu.activeInHierarchy)
        {
            craftingMenu.SetActive(false);
            crafting = false;
            first = true;
        }
        else
        {
            craftingMenu.SetActive(true);
            crafting = true;
        }
    }

    private void setupCraftingUI(Dictionary<string, List<GameObject>> inventory)
    {
        Transform materials = craftingMenu.transform.Find("Materials");

        bool first = true;
        foreach (Transform child in materials)
        {
            List<GameObject> list;
            if (inventory.TryGetValue(child.name.ToUpper(), out list))
            {
                if (list.Count > 0)
                {
                    if (first)
                    {
                        first = false;
                        craftingSelected = child;
                        child.gameObject.GetComponent<Button>().Select();
                    }
                    if (!itemsDiscovered.Contains(child.name))
                    {
                        itemsDiscovered.Add(child.name);
                        child.gameObject.SetActive(true);
                    }
                }
                child.Find("Count").GetComponent<Text>().text = list.Count.ToString();
            }
        }

        Transform craftables = craftingMenu.transform.Find("Craftables");
        List<GameObject> iron, aluminum, mercury, silver, magnesium, cesium, copper, sulpher, zinc;
        foreach (Transform child in craftables)
        {
            switch (child.name)
            {
                case "Thermite":
                    inventory.TryGetValue("IRON", out iron);
                    inventory.TryGetValue("ALUMINUM", out aluminum);
                    if (iron.Count > 0 && aluminum.Count > 0)
                    {
                        child.gameObject.GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.GetComponent<Button>().interactable = false;
                    }
                    break;
                case "Battery":
                    inventory.TryGetValue("SULPHUR", out sulpher);
                    inventory.TryGetValue("ZINC", out zinc);
                    if (sulpher.Count > 0 && zinc.Count > 0)
                    {
                        child.gameObject.GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);

                    }
                    else
                    {
                        child.gameObject.GetComponent<Button>().interactable = false;
                    }
                    break;
                case "Copper Wire":
                    inventory.TryGetValue("COPPER", out copper);
                    if (copper.Count > 0)
                    {
                        child.gameObject.GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.gameObject.GetComponent<Button>().interactable = false;
                    }
                    break;
            }
        }
    }
    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            quitButton.GetComponent<Button>().Select();
            Time.timeScale = 1f;
            paused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            resumeButton.GetComponent<Button>().Select();
            Time.timeScale = 0f;
            paused = true;
        }
    }

    public bool isPaused { get { return paused; } }
    public bool isCrafting { get { return crafting; } }

    public void CraftableClick()
    {
        String clickName = EventSystem.current.currentSelectedGameObject.name;
        Dictionary<string, List<GameObject>> inventory = BM.inventory;
        List<GameObject> list;
        GameObject firstItem;

        switch (clickName)
        {
            case "Thermite":
                inventory.TryGetValue("IRON", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                inventory.TryGetValue("ALUMINUM", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                //TODO Add Thermite
                break;
            case "Battery":
                inventory.TryGetValue("SULPHUR", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                inventory.TryGetValue("ZINC", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);
                //TODO Add Battery

                break;
            case "Copper Wire":
                inventory.TryGetValue("COPPER", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);
                //TODO Add Copper Wire

                break;
        }
        BM.inventory = inventory;
    }

    bool first = true;
    public void MaterialsClick()
    {
        if (first)
        {
            first = false;
        }
        else
        {
            String clickName = EventSystem.current.currentSelectedGameObject.name;
            EM.switchIndex(clickName);
            ToggleCraftingMenu();
            TogglePauseMenu();
        }
    }

    public void Controls()
    {
        //Control menu
    }

    public void Reset()
    {
        TogglePauseMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
