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

    void Start()
    {
        loadingText = GameObject.Find("LoadingText").GetComponent<Text>();
        menu = GameObject.Find("Menu");
        loading = GameObject.Find("Loading");

        loading.SetActive(false);
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
        menu.SetActive(false);
        loading.SetActive(true);
        loadScene = true;
        StartCoroutine(LoadNewScene());
        //SceneManager.LoadScene();
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

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Options()
    {

    }
}
