using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Public
    public event EventHandler<string> Save_Event;
    public event EventHandler<string> Load_Event;
    [HideInInspector]
    public float loadProgress;

    //Private
    private static GameManager instance;
    //private Inventory inventory;
    //private StatsController playerStats;
    private string lastSceneName;
    private List<Methods.SceneLoaderPlayerPosition> playerPositionScenes = new List<Methods.SceneLoaderPlayerPosition>();
    private static List<GameObject> torchList = new List<GameObject>();
    private string nextSceneToLoad = "";
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        FloatingTextController.Initialize();
        
        BuildPlayerPositionList();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    private void FixedUpdate()
    {
        RunTorchesLifeTime();
    }

    private void RunTorchesLifeTime()
    {
        foreach (GameObject torch in torchList)
        {
            Torch torchObj = torch.gameObject.GetComponent<Torch>();
            torchObj.SetLifeTime(Time.deltaTime);

            if (torchObj.GetLifeTime() <= 0)
            {
                torchList.Remove(torch);
                Destroy(torch);
            }
        }
    }

    /*public void SavePlayerInventory()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        /*inventory.SetCurrentTorches(playerInventory.GetCurrentTorches());
        inventory.SetCurrentSnaps(playerInventory.GetCurrentSnaps());

        if (playerInventory.HasKey())
        {
            inventory.GetKey();
        }
    }

    public Inventory LoadPlayerInventory()
    {
        return inventory;
    }

    public void SavePlayerStats()
    {
        playerStats = GameObject.Find("Player").GetComponent<StatsController>();
    }

    public StatsController LoadPlayerStats()
    {
        return playerStats;
    }*/

    public void SaveLastSceneName(string _scene)
    {
        lastSceneName = _scene;
    }

    public string GetLastSceneName()
    {
        return lastSceneName;
    }

    private void BuildPlayerPositionList()
    {
        playerPositionScenes.Add(new Methods.SceneLoaderPlayerPosition(new Vector2(-1.6f, 1.4f), "", "M1L1"));
        playerPositionScenes.Add(new Methods.SceneLoaderPlayerPosition(new Vector2(4, 2), "M1L2", "M1L1"));
        playerPositionScenes.Add(new Methods.SceneLoaderPlayerPosition(new Vector2(-7.5f, -16), "M1L1", "M1L2"));
    }

    public Vector2 GetPlayerPositionAtScene(string _current, string _previews)
    {
        foreach (Methods.SceneLoaderPlayerPosition item in playerPositionScenes)
        {
            if (item.currentMap == _current && item.previwsMap == _previews)
            {
                return item.playerPosition;
            }
        }

        return Vector2.zero;
    }

    public void CallEventLoad(string _sceneName)
    {
        Load_Event?.Invoke(this, _sceneName);
    }

    public void CallEventSave(string _sceneName)
    {
        Save_Event?.Invoke(this, _sceneName);        
    }

    public void AddTorch(GameObject _newTorch)
    {
        torchList.Add(_newTorch.gameObject);
    }

    public void RemoveTorch(GameObject _newTorch)
    {
        torchList.Remove(_newTorch.gameObject);
    }

    public List<GameObject> GetTorchList()
    {
        return torchList;
    }

    public void ClearTorchList()
    {
        foreach (GameObject torch in torchList)
        {
            torch.GetComponent<Animator>().enabled = false;
            Destroy(torch);
        }

        torchList.Clear();
    }

    public void LoadGame(string _slot)
    {
        GameObject.Find("LoadingScreen").GetComponent<SceneLoader>().LoadingScreen();

        loadProgress = 0.1f;

        XMLController.SetSaveSlot(_slot);
        SaveLastSceneName("");
        XMLController.LoadGame();

        loadProgress = 0.3f;

        ClearTorchList();

        loadProgress = 0.4f;

        SceneManager.LoadScene(XMLController.GetSceneFromSlot());        
    }

    public void LoadNextScene(string _nextScene)
    {
        loadProgress = 0.1f;

        Save_Event += LoadNextSceneP2;
        nextSceneToLoad = _nextScene;

        //We will procede when everybody finishes saving their information
        CallEventSave(SceneManager.GetActiveScene().name);
    }

    private void LoadNextSceneP2(object sender, string currentScene)
    {
        loadProgress = 0.3f;

        Save_Event -= LoadNextSceneP2;
        SaveLastSceneName(SceneManager.GetActiveScene().name);
        ClearTorchList();

        loadProgress = 0.4f;

        SceneManager.LoadScene(nextSceneToLoad);
        nextSceneToLoad = "";
    }

    public void SetPlayerPosition()
    {
        string lastLevel = GetLastSceneName() == null ? "" : GetLastSceneName();
        string currentLevel = SceneManager.GetActiveScene().name == null ? "" : SceneManager.GetActiveScene().name;

        if (lastLevel != currentLevel)
        {
            GameObject player = GameObject.Find("Player");
            player.transform.position = GetPlayerPositionAtScene(currentLevel, lastLevel);
        }
    }
}
