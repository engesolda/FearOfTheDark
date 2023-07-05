using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doors : MonoBehaviour
{
    //Public
    public List<AudioClip> audios;

    //Private
    private AudioSource myAudio;
    private bool opened = false;

    private void Start()
    {
        myAudio = gameObject.AddComponent<AudioSource>();

        CheckIfOpenedBefore();

        GameManager gameManager = GameManager.GetInstance();
        gameManager.Save_Event += GameManager_Save_Event;
        gameManager.Load_Event += GameManager_Load_Event;
    }
    
    private void GameManager_Load_Event(object sender, string e)
    {
        List<XMLController.XmlNodeAttributes> attributes = new List<XMLController.XmlNodeAttributes>();
        attributes = XMLController.LoadObjectState(SceneManager.GetActiveScene().name, "Doors", "Door", gameObject.name);

        foreach (XMLController.XmlNodeAttributes attribute in attributes)
        {
            if (attribute.title.Equals("opened"))
            {
                if (Methods.ConvertStringToBool(attribute.value))
                {
                    OpenDoor();
                }
            }
        }
    }

    private void GameManager_Save_Event(object sender, string e)
    {
        List<XMLController.XmlNodeAttributes> attributes = new List<XMLController.XmlNodeAttributes>();
        attributes.Add(new XMLController.XmlNodeAttributes("id", gameObject.name));
        attributes.Add(new XMLController.XmlNodeAttributes("opened", Methods.ConvertBoolToString(opened)));

        XMLController.SaveObjectState(SceneManager.GetActiveScene().name, "Doors", "Door", gameObject.name, attributes);
    }

    private void OnDestroy()
    {
        if (opened)
        {
            PlayerPrefs.SetInt(gameObject.GetInstanceID().ToString(), Methods.ConvertBoolToInt(opened));
        }

        GameManager gameManager = GameManager.GetInstance();
        gameManager.Save_Event -= GameManager_Save_Event;
        gameManager.Load_Event -= GameManager_Load_Event;
    }

    private void CheckIfOpenedBefore()
    {
        bool chestOpen = false;
        List<XMLController.XmlNodeAttributes> attributes = XMLController.LoadObjectState(SceneManager.GetActiveScene().name, "Doors", "Doors", gameObject.name);

        foreach (XMLController.XmlNodeAttributes attribute in attributes)
        {
            if (attribute.title.Equals("opened"))
            {
                chestOpen = Methods.ConvertStringToBool(attribute.value);
            }
        }
        
        if (chestOpen)
        {
            opened = true;
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Equals("Player"))
        {
            if (collision.collider.GetComponent<Player>().HasKey())
            {
                myAudio.clip = audios.Find(clip => clip.name.Equals("DoorOpen"));
                InvokeRepeating("FadeDoor", 0, .1f);
                collision.collider.GetComponent<Player>().UseKey();
            }
            else
            {
                myAudio.clip = audios.Find(clip => clip.name.Equals("DoorLocked"));
            }

            if (!myAudio.isPlaying)
            {
                myAudio.Play();
            }            
        }
    }

    private void FadeDoor()
    {
        Color doorColor = gameObject.GetComponent<SpriteRenderer>().color;

        if (doorColor.a > 0)
        {
            doorColor.a -= 0.1f;
            gameObject.GetComponent<SpriteRenderer>().color = doorColor;
        }
        else
        {
            opened = true;
            gameObject.SetActive(false);
        }
    }

    private void OpenDoor()
    {
        Color doorColor = gameObject.GetComponent<SpriteRenderer>().color;
        doorColor.a = 0;
        gameObject.GetComponent<SpriteRenderer>().color = doorColor;
        opened = true;
        gameObject.SetActive(false);
    }
}
