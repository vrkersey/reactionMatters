using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _mainMenuController : MonoBehaviour {

    private int scene = 1;
    private bool loadScene = false;
    private Text loadingText;
    private GameObject menu;
    private GameObject loading;
    private GameObject savedGame;

    void Start()
    {
        loadingText = GameObject.Find("LoadingText").GetComponent<Text>();
        menu = GameObject.Find("Menu");
        loading = GameObject.Find("Loading");
        savedGame = GameObject.Find("Saved Game");
        loading.SetActive(false);
        if (savedGame == null)
            GameObject.Find("Load").SetActive(false);
    }

    void Update()
    {
        if (loadScene)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
        if (Input.GetButtonDown("BButton"))
            QuitGame();
    }

    public void StartGame()
    {
        if (savedGame != null)
            Destroy(savedGame);
        LoadGame();
    }

    private IEnumerator LoadNewScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("master_level");
        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void LoadGame()
    {
        menu.SetActive(false);
        loading.SetActive(true);
        loadScene = true;
        StartCoroutine(LoadNewScene());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Options()
    {

    }
}
