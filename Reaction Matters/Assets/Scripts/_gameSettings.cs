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

    private float startTime;
    private float timeRemaining;
    private Text timeDisplay;
    private GameObject timeBar;
    private _movementControls MC;
    private _elementMenu EM;
    private _buttonControls BM;

    private Transform resumeButton;
    private Transform quitButton;
    private bool paused = true;
    private bool crafting = false;
    private bool starting = true;
    private List<string> itemsDiscovered = new List<string>();
    private Transform craftingSelected;
    private bool o2cooldown = false;
    private Player savedPlayer;
    private AudioSource breathing;
    private bool blink = false;
    private GameObject warnings;
    private List<GameObject> spawnItems;
    private RawImage hint;
    private Text hintText;
    private GameObject inventoryUI;
    private Image Fade;

    // Use this for initialization
    void Start () {
        startTime = startTimeInMinutes * 60;
        oxygenRefilTimeInMinutes *= 60;
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();
        timeBar = GameObject.Find("TimeBar");
        breathing = GameObject.Find("Heavy Breathing").GetComponent<AudioSource>();
        warnings = GameObject.Find("Warnings");
        warnings.SetActive(false);
        hint = GameObject.Find("Hint").GetComponent<RawImage>();
        hintText = GameObject.Find("HintText").GetComponent<Text>();
        inventoryUI = GameObject.Find("Inventory");
        Fade = GameObject.Find("Fade").GetComponent<Image>();
        Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, 1);

        MC = GameObject.Find("_Main Character").GetComponentInChildren<_movementControls>();
        BM = GameObject.Find("_Main Character").GetComponent<_buttonControls>();
        EM = pauseMenu.transform.Find("Elements").GetComponentInChildren<_elementMenu>();

        resumeButton = pauseMenu.transform.Find("Menu").Find("Resume");
        quitButton = pauseMenu.transform.Find("Menu").Find("Quit");
        spawnItems = new List<GameObject>();

        foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup")){
            pickup.GetComponent<_itemScript>().SpawnItem = true;
            spawnItems.Add(pickup);
        }
        Save();
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.timeSinceLevelLoad < 3f)
        {
            Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, 1 - Time.timeSinceLevelLoad / 2);
            return;
        }else if (starting)
        {
            starting = false;
            paused = false;
        }

        timeRemaining = startTime > 0 ? startTime - Time.fixedTime : 0;
        int minutes = (int)timeRemaining / 60;
        int seconds = (int)timeRemaining % 60;

        string textTime = string.Format("{0:00}   {1:00}", minutes, seconds);
        timeDisplay.text = textTime;
        timeBar.transform.localScale = new Vector3(timeRemaining / (15*60) > 1 ? 1 : timeRemaining / (15 * 60), timeBar.transform.localScale.y, timeBar.transform.localScale.z);
        if (timeRemaining <= 63)
            gameObject.SendMessage("playLastMinute");

        if (timeRemaining <= 0)
        {
            Reset();
            breathing.Stop();
            return;
        }
        else if (timeRemaining <= 20)
        {
            if (!breathing.isPlaying)
                breathing.Play();
            breathing.volume = 1 - timeRemaining / 20;
        }
        else if (timeRemaining >= 20 && breathing.isPlaying)
        {
            breathing.Stop();
        }
        
        if (timeRemaining <= 60 && !blink)
        {
            blink = true;
            StartCoroutine(Blink(warnings, .5f));
            gameObject.SendMessage("playVoice", "Low_Oxygen");
        }
        else if (timeRemaining > 60)
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
                startTime += oxygenRefilTimeInMinutes;
                StartCoroutine(Cooldown());
            }
        }
    }

    void blankHints()
    {
        hint.color = new Color(255, 255, 255, 0);
        hintText.text = "";
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
            inventoryUI.SetActive(true);
            inventoryUI.SendMessage("updateInventory");
        }
        else
        {
            craftingMenu.SetActive(true);
            crafting = true;
            blankHints();
            inventoryUI.SetActive(false);
        }
    }
    IEnumerator SelectContinueButtonLater(GameObject button)
    {
        yield return null;
        gameObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        gameObject.GetComponent<EventSystem>().SetSelectedGameObject(button);
    }

    private void setupCraftingUI(Dictionary<string, List<GameObject>> inventory)
    {
        Transform materials = craftingMenu.transform.Find("Materials");
        StartCoroutine(SelectContinueButtonLater(materials.Find("Aluminum").Find("Button").gameObject));

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
            blankHints();
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

        //make a deep copy of inventory 
        current.inventory = new Dictionary<string, List<GameObject>>();
        foreach (KeyValuePair<string, List<GameObject>> entry in BM.GetComponent<_buttonControls>().inventory)
            current.inventory.Add(entry.Key, new List<GameObject>(entry.Value));

        savedPlayer = current;
    }

    public void Load()
    {
        float addTime = timeRemaining > startTimeInMinutes * 60 ? 0 : startTimeInMinutes * 60 - timeRemaining;
        startTime += addTime;
        GameObject player = GameObject.Find("_Main Character");
        GameObject mover = player.transform.Find("Mover").gameObject;

        mover.SendMessage("resetRotation");
        player.transform.position = savedPlayer.position;
        mover.transform.rotation = savedPlayer.rotation;

        BM.GetComponent<_buttonControls>().inventory = new Dictionary<string, List<GameObject>>();
        foreach (KeyValuePair<string, List<GameObject>> entry in savedPlayer.inventory)
            BM.GetComponent<_buttonControls>().inventory.Add(entry.Key, new List<GameObject>(entry.Value));

        foreach (GameObject spawnItem in spawnItems)
            spawnItem.SetActive(true);
    }

    public void Controls()
    {
        //Control menu
    }

    public void Reset()
    {
        Load();
        TogglePauseMenu();
    }

    public void Quit()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}

public struct Player
{
    public Vector3 position;
    public Quaternion rotation;
    public Dictionary<string, List<GameObject>> inventory;
}
