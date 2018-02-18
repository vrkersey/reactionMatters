using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class _gameSettings : MonoBehaviour {

    [Space(10)]
    [Header("Level things")]
    public GameObject pauseMenu;
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
            quitButton.GetComponent<Button>().Select();
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            resumeButton.GetComponent<Button>().Select();
            Time.timeScale = 0f;
        }
    }

    public void ReallyDoNothing()
    {

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
