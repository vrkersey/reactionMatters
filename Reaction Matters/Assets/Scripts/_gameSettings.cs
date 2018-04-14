using System;
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
    public float oxygenRefilTimeInMinutes = 3f;
    public float itemRespawnTimeInMinutes = 3f;
   
    [Tooltip("Time in Seconds to Fill Tool from 0% to 100%")]
    public float toolRespawnTime = 100f;
    [Tooltip("Time in Seconds to Use Tool from 100% to 0%")]
    public float toolUseTime = 20f;
    [Space(10)]
    [Header("Player things")]
    public float movementSpeed = 15f;
    public float sensitivity = 15f;
    public float jumpHeight = 2.5f;
    [Space(10)]
    [Header("Craftable Items Prefabs")]
    public GameObject Thermite;
    public GameObject Battery;
    public GameObject Copper_Wire;

    private float timeRemaining;
    private Text timeDisplay;
    private GameObject timeBar;
    private _movementControls MC;
    private _elementMenu EM;
    private _buttonControls BM;

    private Transform resumeButton;
    private Transform quitButton;
    private bool paused = false;
    private bool crafting = false;
    private List<string> itemsDiscovered = new List<string>();
    private Transform craftingSelected;
    private bool o2cooldown;
    private Player savedPlayer;
    private AudioSource breathing;
    private bool blink = false;
    private GameObject warnings;

    // Use this for initialization
    void Start () {
        timeRemaining = startTimeInMinutes * 60;
        oxygenRefilTimeInMinutes *= 60;
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();
        timeBar = GameObject.Find("TimeBar");
        breathing = GameObject.Find("Heavy Breathing").GetComponent<AudioSource>();
        warnings = GameObject.Find("Warnings");
        warnings.SetActive(false);

        MC = GameObject.Find("_Main Character").GetComponentInChildren<_movementControls>();
        BM = GameObject.Find("_Main Character").GetComponent<_buttonControls>();
        EM = pauseMenu.transform.Find("Elements").GetComponentInChildren<_elementMenu>();

        resumeButton = pauseMenu.transform.Find("Menu").Find("Resume");
        quitButton = pauseMenu.transform.Find("Menu").Find("Quit");

        foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup")){
            pickup.GetComponent<_itemScript>().SpawnItem = true;
        }
        Save();
    }
	
	// Update is called once per frame
	void Update () {
        float guiTime = timeRemaining > 0 ? timeRemaining - Time.fixedTime : 0;
        int minutes = (int)guiTime / 60;
        int seconds = (int)guiTime % 60;

        string textTime = string.Format("{0:00}   {1:00}", minutes, seconds);
        timeDisplay.text = textTime;
        timeBar.transform.localScale = new Vector3(timeRemaining/(15*60) > 1 ? 1 : timeRemaining / (15 * 60), timeBar.transform.localScale.y, timeBar.transform.localScale.z);
        if (guiTime == 0)
        {
            timeRemaining += startTimeInMinutes * 60;
            Reset();
        }
        else if (guiTime <= 20)
        {
            if (!breathing.isPlaying)
                breathing.Play();
            breathing.volume = (20 - guiTime) / 20;
        }
        else if (guiTime >= 20 && breathing.isPlaying)
        {
            breathing.Stop();
        }
        
        if (guiTime <= 60 && !blink)
        {
            blink = true;
            StartCoroutine(Blink(warnings, .5f));
        }
        else if (guiTime > 60)
        {
            blink = false;
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
            if (Input.GetButtonDown("YButton") && !o2cooldown)
            {
                timeRemaining += oxygenRefilTimeInMinutes;
                StartCoroutine(Cooldown());
            }
        }
    }
    IEnumerator Blink(GameObject image, float interval)
    {
        while (blink)
        {
            if (image.activeSelf)
            {
                image.SetActive(false);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                image.SetActive(true);
                yield return new WaitForSeconds(interval);
            }
        }
        image.SetActive(false);
    }
    IEnumerator Cooldown()
    {
        o2cooldown = true;
        yield return new WaitForSeconds(oxygenRefilTimeInMinutes + 30);
        o2cooldown = false;
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
                        child.Find("Button").GetComponent<Button>().Select();
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
        List<GameObject> iron, aluminum, mercury, silver, magnesium, cesium, copper, lithium, manganese;
        foreach (Transform child in craftables)
        {
            switch (child.name)
            {
                case "Thermite":
                    inventory.TryGetValue("IRON", out iron);
                    inventory.TryGetValue("ALUMINUM", out aluminum);
                    if (iron.Count > 0 && aluminum.Count > 0)
                    {
                        child.Find("Button").GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.Find("Button").GetComponent<Button>().interactable = false;
                    }
                    break;
                case "Battery":
                    inventory.TryGetValue("LITHIUM", out lithium);
                    inventory.TryGetValue("MANGANESE", out manganese);
                    if (lithium.Count > 0 && manganese.Count > 0)
                    {
                        child.Find("Button").GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);

                    }
                    else
                    {
                        child.Find("Button").GetComponent<Button>().interactable = false;
                    }
                    break;
                case "Copper Wire":
                    inventory.TryGetValue("COPPER", out copper);
                    if (copper.Count > 0)
                    {
                        child.Find("Button").GetComponent<Button>().interactable = true;
                        child.gameObject.SetActive(true);
                    }
                    else
                    {
                        child.Find("Button").GetComponent<Button>().interactable = false;
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
        String clickName = EventSystem.current.currentSelectedGameObject.transform.parent.name;

        Dictionary<string, List<GameObject>> inventory = BM.inventory;
        List<GameObject> list;
        GameObject firstItem;
        GameObject newObject;

        switch (clickName)
        {
            case "Thermite":
                // Remove items
                inventory.TryGetValue("IRON", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                inventory.TryGetValue("ALUMINUM", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                // Add Thermite
                newObject = (GameObject)GameObject.Instantiate(Thermite, Vector3.zero, Quaternion.identity);
                inventory.TryGetValue("THERMITE", out list);
                list.Add(newObject);
                
                break;
            case "Battery":
                // Remove items
                inventory.TryGetValue("LITHIUM", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                inventory.TryGetValue("MANGANESE", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                // Add Battery
                newObject = (GameObject)GameObject.Instantiate(Battery, Vector3.zero, Quaternion.identity);
                inventory.TryGetValue("BATTERY", out list);
                list.Add(newObject);

                break;
            case "Copper Wire":
                // Remove items
                inventory.TryGetValue("COPPER", out list);
                firstItem = list[0];
                Destroy(firstItem);
                list.Remove(firstItem);

                // Add Copper Wire
                newObject = (GameObject)GameObject.Instantiate(Copper_Wire, Vector3.zero, Quaternion.identity);
                inventory.TryGetValue("COPPER_WIRE", out list);
                list.Add(newObject);

                break;
        }
        BM.inventory = inventory;
        setupCraftingUI(inventory);
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
            String clickName = EventSystem.current.currentSelectedGameObject.transform.parent.name;
            EM.switchIndex(clickName);
            ToggleCraftingMenu();
            TogglePauseMenu();
        }
    }

    public void Save()
    {
        GameObject player = GameObject.Find("_Main Character");

        Player current = new Player();
        current.position = player.transform.position;
        current.rotation = player.transform.Find("Mover").rotation;
        current.inventory = new Dictionary<string, List<GameObject>>();

        //make a deep copy of inventory 
        foreach (KeyValuePair<string, List<GameObject>> entry in BM.GetComponent<_buttonControls>().inventory)
            current.inventory.Add(entry.Key, new List<GameObject>(entry.Value));

        savedPlayer = current;
    }

    public void Load()
    {
        GameObject player = GameObject.Find("_Main Character");
        GameObject mover = player.transform.Find("Mover").gameObject;

        mover.SendMessage("resetRotation");
        player.transform.position = savedPlayer.position;
        mover.transform.rotation = savedPlayer.rotation;
        BM.GetComponent<_buttonControls>().inventory = savedPlayer.inventory;
    }

    public void Controls()
    {
        //Control menu
    }

    public void Reset()
    {
        TogglePauseMenu();
        Load();
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}

public struct Player
{
    public Vector3 position;
    public Quaternion rotation;
    public Dictionary<string, List<GameObject>> inventory;
}
