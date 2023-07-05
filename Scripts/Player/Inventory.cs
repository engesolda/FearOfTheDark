using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO - make the torches created "invisible" on hierarchy

public class Inventory : MonoBehaviour
{
    //Public
    
    public int maxTorches;
    public int maxSnaps;

    //Private
    private int currentTorches = 0;
    private int currentSnaps = 0;
    private bool hasKey = false;
    private GameObject key;
    private Text txtTorches;
    private Text txtSnaps;

    // Start is called before the first frame update
    void Start()
    {
        txtTorches = GameObject.Find("TrochAmountTxt").GetComponent<Text>();
        txtSnaps = GameObject.Find("SnapAmountTxt").GetComponent<Text>();
        key = GameObject.Find("UI_KeyImage") as GameObject;
    }

    public void Initialize(Inventory _loadedInventory)
    {
        txtTorches = GameObject.Find("TrochAmountTxt").GetComponent<Text>();
        txtSnaps = GameObject.Find("SnapAmountTxt").GetComponent<Text>();
        key = GameObject.Find("UI_KeyImage") as GameObject;

        if (_loadedInventory != null)
        {
            currentTorches = _loadedInventory.currentTorches;
            currentSnaps = _loadedInventory.currentSnaps;
            if (_loadedInventory.hasKey)
            {
                GetKey();
            }
        }
        else
        {
            currentTorches = 0;
            currentSnaps = 0;
            hasKey = false;
        }

        txtTorches.text = currentTorches.ToString();
        txtSnaps.text = currentSnaps.ToString();
    }

    public void AddTorch(int _count)
    {
        int finalNumber = currentTorches + _count;
        string difference = "+" + (finalNumber - currentTorches).ToString();

        if (finalNumber <= maxTorches)
        {
            if (finalNumber > currentTorches)
            {
                FloatingTextController.CreateFloatingText(txtTorches.transform.position, difference, Color.green, 30);
            }
            else
            {
                FloatingTextController.CreateFloatingText(txtTorches.transform.position, difference, Color.red, 30);
            }
            currentTorches = finalNumber;
        }
        if (finalNumber > maxTorches)
        {
            currentTorches = maxTorches;
            FloatingTextController.CreateFloatingText(txtTorches.transform.position, difference, Color.green, 30);
        }
        if (finalNumber < 0)
        {
            currentTorches = 0;
            FloatingTextController.CreateFloatingText(txtTorches.transform.position, difference, Color.red, 30);
        }

        txtTorches.text = currentTorches.ToString();
    }

    public void AddSnap(int _count)
    {
        int finalNumber = currentSnaps + _count;
        string difference = "+" + (finalNumber - currentSnaps).ToString();

        if (finalNumber <= maxSnaps)
        {
            if (finalNumber > currentSnaps)
            {
                FloatingTextController.CreateFloatingText(txtSnaps.transform.position, difference, Color.green, 30);
            }
            else
            {
                FloatingTextController.CreateFloatingText(txtSnaps.transform.position, difference, Color.red, 30);
            }
            currentSnaps = finalNumber;
        }
        if (finalNumber > maxSnaps)
        {
            currentSnaps = maxSnaps;
            FloatingTextController.CreateFloatingText(txtSnaps.transform.position, difference, Color.green, 30);
        }
        if (finalNumber < 0)
        {
            currentSnaps = 0;
            FloatingTextController.CreateFloatingText(txtSnaps.transform.position, difference, Color.red, 30);
        }

        txtSnaps.text = currentSnaps.ToString();
    }

    public bool CanPlaceTorch()
    {
        if (currentTorches > 0)
        {
            return true;
        }

        return false;
    }

    public bool CanFireSnap()
    {
        if (currentSnaps > 0)
        {
            return true;
        }

        return false;
    }

    public void GetKey()
    {
        hasKey = true;
        key.GetComponent<Image>().enabled = true;
    }

    public void UseKey()
    {
        hasKey = false;
        key.GetComponent<Image>().enabled = false;
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public List<XMLController.XmlNodeAttributes> SaveInventory()
    {
        List<XMLController.XmlNodeAttributes> stats = new List<XMLController.XmlNodeAttributes>();

        stats.Add(new XMLController.XmlNodeAttributes("id", "inventory"));
        stats.Add(new XMLController.XmlNodeAttributes("maxTorches", maxTorches.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("maxSnaps", maxSnaps.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("currentTorches", currentTorches.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("currentSnaps", currentSnaps.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("hasKey", Methods.ConvertBoolToString(hasKey)));

        return stats;
    }

    public void LoadInventory(List<XMLController.XmlNodeAttributes> _attributes)
    {
        foreach (XMLController.XmlNodeAttributes attribute in _attributes)
        {
            if (attribute.title.Equals("maxTorches"))
            {
                maxTorches = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("maxSnaps"))
            {
                maxSnaps = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("currentTorches"))
            {
                currentTorches = int.Parse(attribute.value);
                txtTorches.text = currentTorches.ToString();
            }
            if (attribute.title.Equals("currentSnaps"))
            {
                currentSnaps = int.Parse(attribute.value);
                txtSnaps.text = currentSnaps.ToString();
            }
            if (attribute.title.Equals("hasKey"))
            {
                if (Methods.ConvertStringToBool(attribute.value))
                {
                    GetKey();
                }
            }
        }
    }
}
