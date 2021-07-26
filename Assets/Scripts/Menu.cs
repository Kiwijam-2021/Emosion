using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string MainSceneName = "Main";
    public string MenuSceneName = "Menu";
    public string LoadingSceneName = "Loading";
    
    public void StartGame()
    {
        SceneManager.LoadScene(LoadingSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}