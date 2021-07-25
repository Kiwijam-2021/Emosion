using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string MainSceneName = "Main";
    public string MenuSceneName = "Menu";
    
    public void StartGame()
    {
        SceneManager.LoadScene(MainSceneName);
        var spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner)
        {
            var script = spawner.GetComponent<Spawner>();
            script.Reconnect();
        }
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