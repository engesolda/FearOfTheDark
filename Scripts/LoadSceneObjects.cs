using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;

public class LoadSceneObjects : MonoBehaviour
{
    //Public
    

    //Private
    private float loadTime = 2;
    private bool loaded = false;
    private GameObject torch;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.GetInstance();

        gameManager.loadProgress = 0.5f;

        torch = Resources.Load("Torch") as GameObject;

        gameManager.SetPlayerPosition();
    }

    private void Update()
    {
        //Loading from one scene to another
        if (!string.IsNullOrEmpty(gameManager.GetLastSceneName()))
        {
            loadTime -= Time.deltaTime;

            if (loadTime <= 0 && !loaded)
            {
                gameManager.loadProgress = 0.7f;

                LoadObjectsSceneToScene();
                loaded = true;
            }
        }
        else //Loading from zero
        {
            loadTime -= Time.deltaTime;

            if (loadTime <= 0 && !loaded)
            {
                gameManager.loadProgress = 0.7f;

                LoadObjectsMenuToScene();
                loaded = true;
            }
        }
    }

    private void LoadObjectsSceneToScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        //First we ask every object that is abble to load itself to do it
        gameManager.CallEventLoad(currentScene);

        //Now we need to load the objects that cant load themselves

        //---Loading player
        Player.GetInstance().InitializePlayer();
        gameManager.loadProgress = 0.8f;

        //---Loading Torches
        List<XmlNode> torchNodes;
        torchNodes = XMLController.LoadSlotObjects("Lights", "Torch");

        if (torchNodes != null)
        {
            foreach (XmlNode node in torchNodes)
            {
                torch = Instantiate(torch) as GameObject;
                torch.GetComponent<Torch>().SetLightAttributes(node.Attributes);

                if (!node.Attributes["scene"].Value.Equals(currentScene))
                {
                    torch.GetComponent<Torch>().SetInvisible();
                }
                else
                {
                    torch.GetComponent<Torch>().SetVisible();
                }
                
                gameManager.AddTorch(torch);
            }
        }
        gameManager.loadProgress = 0.9f;

        gameManager.SetPlayerPosition();
        gameManager.loadProgress = 1f;
    }

    private void LoadObjectsMenuToScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        //First we ask every object that is abble to load itself to do it
        gameManager.CallEventLoad(currentScene);

        //Now we need to load the objects that cant load themselves

        //---Loading Player
        Player.GetInstance().LoadPlayer();
        gameManager.loadProgress = 0.8f;

        //---Loading Torches
        //We need to load all torches in the game into the list in gameManager to countdown their lifetime
        List<XmlNode> torchNodes = XMLController.LoadSlotObjects("Lights", "Torch");

        if (torchNodes != null)
        {
            foreach (XmlNode node in torchNodes)
            {
                torch = Instantiate(torch) as GameObject;
                torch.GetComponent<Torch>().SetLightAttributes(node.Attributes);

                if (!node.Attributes["scene"].Value.Equals(currentScene))
                {
                    torch.GetComponent<Torch>().SetInvisible();
                }

                gameManager.AddTorch(torch);
            }
        }
        gameManager.loadProgress = 1f;
    }
}