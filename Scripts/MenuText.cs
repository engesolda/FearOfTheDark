using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuText : MonoBehaviour
{
    //Public 
    public GameMenuController menuController;
    public GameObject nextMenu;
    public GameObject previewsMenu;
    public Image savedImage;
    public Text savedText;

    //Private
    [SerializeField]private int myIndex;
    private Animator myAnimator;
    private AudioSource audioSource;
    private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        audioSource = menuController.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (menuController.index == myIndex)
        {
            myAnimator.SetBool("Selected", true);

            if (Input.GetKeyDown("return"))
            {
                myAnimator.SetTrigger("Clicked");
            }
        }
        else
        {
            myAnimator.SetBool("Selected", false);
        }
    }

    void OpenNextMenu()
    {
        string parentMenu = transform.parent.gameObject.name;

        switch (parentMenu)
        {
            case "GameMenuPanel":
                GameMenuPanel();
                break;

            case "GameMenuSave":
                GameMenuSave();
                break;

            case "GameMenuLoad":
                GameMenuLoad();
                break;
        }                
    }

    private void GameMenuPanel()
    {
        switch (myIndex)
        {
            case 0: //Close Pause Menu
                Time.timeScale = 1;
                previewsMenu.SetActive(false);
                break;

            case 3: //Save
                nextMenu.SetActive(true);
                previewsMenu.SetActive(false);
                menuController.index = 1;
                menuController.numMenus = 4;
                break;

            case 4: //Load
                nextMenu.SetActive(true);
                previewsMenu.SetActive(false);
                menuController.index = 1;
                menuController.numMenus = 4;
                break;
        }
    }

    private void GameMenuSave()
    {
        switch (myIndex)
        {
            case 0: //Back
                nextMenu.SetActive(true);
                menuController.numMenus = 6;
                menuController.index = 3;
                previewsMenu.SetActive(false);                
                break;

            case 1://Save slot 1
                SaveGame("SaveSlot1");
                break;

            case 2://Save slot 2
                SaveGame("SaveSlot2");
                break;

            case 3://Save slot 3
                SaveGame("SaveSlot3");
                break;
        }
    }

    private void GameMenuLoad()
    {
        GameManager gameManager = GameManager.GetInstance();

        switch (myIndex)
        {
            case 0: //Back
                nextMenu.SetActive(true);
                menuController.numMenus = 6;
                menuController.index = 4;
                previewsMenu.SetActive(false);
                break;

            case 1://Save slot 1
                Time.timeScale = 1;
                gameManager.LoadGame("SaveSlot1");
                break;

            case 2://Save slot 2
                Time.timeScale = 1;
                gameManager.LoadGame("SaveSlot2");
                break;

            case 3://Save slot 3
                Time.timeScale = 1;
                gameManager.LoadGame("SaveSlot3");
                break;
        }
    }

    private void SaveGame(string _saveSlot)
    {
        GameManager gameManager = GameManager.GetInstance();
        GameObject mainCamera = GameObject.Find("Main Camera");
        string sceneName = SceneManager.GetActiveScene().name;

        //First I set the saveSlot on XMLController cause the screenshot needs this
        XMLController.SetSaveSlot(_saveSlot);

        string imgPath = mainCamera.GetComponent<HiResScreenShots>().TakeScreenShot();
        string[] imgName = imgPath.Split('/');
        imgPath = "Assets/screenshots/" + imgName[imgName.Length - 1];

        //Load the asset asap cause can take a while
        AssetDatabase.ImportAsset(imgPath);   
        XMLController.ClearTemporaryScene(sceneName);
        gameManager.CallEventSave(sceneName);
        XMLController.SaveGame(sceneName, sceneName);

        //Now the game is saved I can change the picture displayed
        Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(imgPath, typeof(Sprite));
        savedImage.sprite = sprite;
    }

    public void PlaySound(AudioClip _sound)
    {
        audioSource.PlayOneShot(_sound);
    }

    public void UpdateSavedImage()
    {
        string buttonName = gameObject.name;
        string imgPath = "Assets/screenshots/";
        string sceneName = SceneManager.GetActiveScene().name;
        string saveSlotName = "";

        if (buttonName.Equals("SaveSlot1"))
        {
            saveSlotName = "SaveSlot1.png";
        }

        if (buttonName.Equals("SaveSlot2"))
        {
            saveSlotName = "SaveSlot2.png";
        }

        if (buttonName.Equals("SaveSlot3"))
        {
            saveSlotName = "SaveSlot3.png";
        }

        if (!string.IsNullOrEmpty(saveSlotName))
        {
            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(imgPath + saveSlotName, typeof(Sprite));
            savedImage.sprite = sprite;

            savedText.text = XMLController.GetSavedSlotText(saveSlotName.Split('.')[0]);
        }
    }
}
