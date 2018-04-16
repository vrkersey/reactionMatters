using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _mainMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        SceneManager.LoadScene("master_level", LoadSceneMode.Single);
    }

    public void LoadGame()
    {

    }

    public void QuitGame()
    {

    }

    public void Options()
    {

    }
}
