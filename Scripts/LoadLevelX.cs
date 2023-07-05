using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelX : MonoBehaviour
{
    //Public
    public string nextSceneName;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Player"))
        {
            GameObject.Find("LoadingScreen").GetComponent<SceneLoader>().LoadingScreen();
            GameManager.GetInstance().LoadNextScene(nextSceneName);            
        }
    }
}
