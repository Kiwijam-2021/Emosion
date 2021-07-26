using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string SceneName = "Main";

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            var res = SceneManager.UnloadSceneAsync(SceneName);
            Debug.Log($"Loading {SceneName}");
            while (!res.isDone)
            {
            }
        }
        catch
        {
        }

        SceneManager.LoadScene(SceneName);

        var spawner = GameObject.FindGameObjectWithTag("Spawner");
        if (spawner)
        {
            var script = spawner.GetComponent<Spawner>();
            script.Reconnect();
        }
    }
}