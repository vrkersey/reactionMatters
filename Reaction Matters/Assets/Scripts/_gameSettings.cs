using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private Transform resumeButton;
    private Transform quitButton;
    private bool paused = false;
    private bool crafting = false;
    private List<string> itemsDiscovered = new List<string>();


    // Use this for initialization
    void Start () {
        timeRemaining = startTimeInMinutes * 60;
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();

        MC = GameObject.Find("_Main Character").GetComponentInChildren<_movementControls>();
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
        ToggleCraftingMenu();
        Transform materials = craftingMenu.transform.Find("Materials");
        
        foreach (Transform child in materials)
        {
            List<GameObject> list;
            if (inventory.TryGetValue(child.name.ToUpper(), out list))
            {
                if (list.Count > 0 && !itemsDiscovered.Contains(child.name))
                {
                    itemsDiscovered.Add(child.name);
                    child.gameObject.SetActive(true);
                }
                child.Find("Count").GetComponent<Text>().text = list.Count.ToString();
            }
        }
    }

    private void ToggleCraftingMenu()
    {
        if (craftingMenu.activeInHierarchy)
        {
            craftingMenu.SetActive(false);
            crafting = false;
        }
        else
        {
            craftingMenu.SetActive(true);
            crafting = true;
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

    public void Controls()
    {
        //Control menu
    }

    public void Reset()
    {
        //reset button
    }

    public void Quit()
    {
        Application.Quit();
    }
}
