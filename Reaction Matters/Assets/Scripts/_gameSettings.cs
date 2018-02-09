using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _gameSettings : MonoBehaviour {

    [Space(10)]
    [Header("Level things")]
    public float startTimeInMinutes = 5f;
    [Space(10)]
    [Header("Player things")]
    public float movementSpeed = 15f;
    public float sensitivity = 15f;

    

    private float timeRemaining;
    private Text timeDisplay;
    private GameObject pauseMenu;
    private _movementControls MC;

	// Use this for initialization
	void Start () {
        timeRemaining = startTimeInMinutes * 60;
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();

        pauseMenu = GameObject.Find("Pause Menu");
        MC = GameObject.Find("Main Character").GetComponentInChildren<_movementControls>();
        TogglePauseMenu();
	}
	
	// Update is called once per frame
	void Update () {
        float guiTime = timeRemaining - Time.fixedTime;
        int minutes = (int)guiTime / 60;
        int seconds = (int)guiTime % 60;

        string textTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        timeDisplay.text = textTime;
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            MC.LockMovement = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            MC.LockMovement = true;
        }
    }

    public void ReallyDoNothing()
    {

    }
}
