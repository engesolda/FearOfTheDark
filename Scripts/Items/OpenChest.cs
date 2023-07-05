using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenChest : MonoBehaviour
{
    //Public 
    public int minQuantity;
    public int maxQuantity;
    public string itemReward;
    //public GameObject player;

    //Private
    private bool opened = false;
    private AudioSource myAudio;
    //private Inventory playerInventory;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.GetInstance();

        gameManager.Save_Event += OpenChest_Save_Event;
        gameManager.Load_Event += OpenChest_Load_Event;

        myAudio = GetComponent<AudioSource>();
        //playerInventory = player.GetComponent<Inventory>();
    }

    private void OpenChest_Load_Event(object sender, string sceneName)
    {
        List<XMLController.XmlNodeAttributes> attributesToAdd;
        attributesToAdd = XMLController.LoadObjectState(sceneName, "Chests", "Chest", gameObject.name);

        if (attributesToAdd != null)
        {
            foreach (XMLController.XmlNodeAttributes attribute in attributesToAdd)
            {
                if (attribute.title == "opened")
                {
                    opened = Methods.ConvertStringToBool(attribute.value);
                }
            }

            SetObjectState();
        }
    }

    private void OpenChest_Save_Event(object sender, string sceneName)
    {
        List<XMLController.XmlNodeAttributes> attributesToAdd = new List<XMLController.XmlNodeAttributes>();

        attributesToAdd.Add(new XMLController.XmlNodeAttributes("id", gameObject.name));
        attributesToAdd.Add(new XMLController.XmlNodeAttributes("opened", Methods.ConvertBoolToString(opened)));

        XMLController.SaveObjectState(sceneName, "Chests", "Chest", gameObject.name, attributesToAdd);
    }

    private void SetObjectState()
    {
        if (opened)
        {
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().Play("ChestOpened");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player") && !opened)
        {
            GetComponent<Animator>().enabled = true;
            GetComponent<ParticleSystem>().Play();
            opened = true;
            CallReward();
            myAudio.Play();
        }
    }

    private void OnDestroy()
    {
        gameManager.Save_Event -= OpenChest_Save_Event;
        gameManager.Load_Event -= OpenChest_Load_Event;
    }

    private void CallReward()
    {
        if (itemReward.Equals("Torch"))
        {
            Player.GetInstance().AddTorch(Random.Range(minQuantity, maxQuantity));
        }
        if (itemReward.Equals("Snaps"))
        {
            Player.GetInstance().AddSnap(Random.Range(minQuantity, maxQuantity));
        }
        if (itemReward.Equals("Key"))
        {
            Player.GetInstance().GetKey();
        }
    }
}
