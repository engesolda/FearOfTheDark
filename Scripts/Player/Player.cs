using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Public
    public float snapSpeed;
    public float timeBewteenSnaps;
    public float timeBetweenDarkHit;    
    public int numIniTorches;
    public int numIniSnaps;
    public Light playerLight;
    public Light flashLight; 
    [HideInInspector]public StatsController myStats;

    //Private
    private bool onLight = false;
    private bool onHealingLight = false;
    private float timeToLaunchSnap = 0;
    private GameObject snaps;
    private GameObject torch;
    private Inventory myInventory;
    private SoundController mySounds;
    private GameManager gameManager;
    private static Player instance;

    // Start is called before the first frame update
    void Start()
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

        mySounds = GetComponent<SoundController>();
        myStats = GetComponent<StatsController>();
        myInventory = GetComponent<Inventory>();
        myStats.Initialize(null);
        myInventory.Initialize(null);
        torch = Resources.Load("Torch") as GameObject;
        snaps = Resources.Load("Snaps") as GameObject;
        playerLight = Instantiate(playerLight, transform);
        flashLight = Instantiate(flashLight, transform);
        gameManager = GameManager.GetInstance();

        gameManager.Save_Event += GameManager_Save_Event;

        InvokeRepeating("CheckIfOnDarkness", 0, timeBetweenDarkHit);
    }

    public static Player GetInstance()
    {
        return instance;
    }

    private void SetPlayerInfo(List<XMLController.XmlNodeAttributes> _attributes)
    {
        foreach (XMLController.XmlNodeAttributes attribute in _attributes)
        {
            if (attribute.title.Equals("position"))
            {
                transform.position = Methods.ConvertStringToVector3(attribute.value);
            }
            if (attribute.title.Equals("snapSpeed"))
            {
                snapSpeed = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("timeBewteenSnaps"))
            {
                timeBewteenSnaps = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("timeBetweenDarkHit"))
            {
                timeBetweenDarkHit = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("numIniTorches"))
            {
                numIniTorches = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("timeToLaunchSnap"))
            {
                timeToLaunchSnap = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("numIniSnaps"))
            {
                numIniSnaps = int.Parse(attribute.value);
            }
        }
    }

    public void InitializePlayer()
    {
        torch = Resources.Load("Torch") as GameObject;
        snaps = Resources.Load("Snaps") as GameObject;
        myStats.Initialize(myStats);
        myInventory.Initialize(myInventory);
    }

    public void LoadPlayer()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        List<XMLController.XmlNodeAttributes> attributesList = new List<XMLController.XmlNodeAttributes>();
        attributesList = XMLController.LoadObjectState(sceneName, "Player", "Info", gameObject.name);
        SetPlayerInfo(attributesList);

        attributesList.Clear();
        attributesList = XMLController.LoadObjectState(sceneName, "Player", "Stats", "stats");
        myStats.LoadStats(attributesList);

        attributesList.Clear();
        attributesList = XMLController.LoadObjectState(sceneName, "Player", "Inventory", "inventory");
        myInventory.LoadInventory(attributesList);
    }

    private void GameManager_Save_Event(object sender, string e)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        List<XMLController.XmlNodeAttributes> attributesList = new List<XMLController.XmlNodeAttributes>();
        attributesList.Add(new XMLController.XmlNodeAttributes("id", gameObject.name));
        attributesList.Add(new XMLController.XmlNodeAttributes("position", transform.position.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("snapSpeed", snapSpeed.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("timeBewteenSnaps", timeBewteenSnaps.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("timeBetweenDarkHit", timeBetweenDarkHit.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("numIniTorches", numIniTorches.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("timeToLaunchSnap", timeToLaunchSnap.ToString()));
        attributesList.Add(new XMLController.XmlNodeAttributes("numIniSnaps", numIniSnaps.ToString()));
        XMLController.SaveObjectState(sceneName, "Player", "Info", gameObject.name, attributesList);

        attributesList.Clear();
        attributesList = myStats.SaveStats();
        XMLController.SaveObjectState(sceneName, "Player", "Stats", "stats", attributesList);

        attributesList.Clear();
        attributesList = myInventory.SaveInventory();
        XMLController.SaveObjectState(sceneName, "Player", "Inventory", "inventory", attributesList);
    }

    private void OnDestroy()
    {
        gameManager.Save_Event -= GameManager_Save_Event;
    }

    private void FixedUpdate()
    {
        CheckForInputs();
    }

    private void CheckIfOnDarkness()
    {
        if (!onLight)
        {
            myStats.ApplyDamageToPlayer(-1);
        }

        if (onHealingLight && myStats.currentHitpoints < myStats.maxHitpoints)
        {
            myStats.ApplyDamageToPlayer(1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = true;

            if (collision.name.Equals("HealingLight"))
            {
                onHealingLight = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = true;

            if (collision.name.Equals("HealingLight"))
            {
                onHealingLight = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = false;

            if (collision.name.Equals("HealingLight"))
            {
                onHealingLight = false;
            }
        }
    }

    public bool IsOnLight()
    {
        return onLight;
    }

    private void CheckForInputs()
    {
        timeToLaunchSnap -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            if (timeToLaunchSnap <= 0 && !myStats.beignDragged && myInventory.CanFireSnap())
            {
                GameObject snap = Instantiate(snaps) as GameObject;
                snap.transform.position = transform.position;
                snap.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * snapSpeed;
                timeToLaunchSnap = timeBewteenSnaps;
                myInventory.AddSnap(-1);
                mySounds.PlayThrowingSnapSound();
            }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            TurnOnOffFlashLight();
        }

        if (Input.GetKeyDown("space") && myInventory.CanPlaceTorch())
        {
            torch = Instantiate(torch, transform.position, transform.rotation);
            torch.transform.SetParent(transform);
            gameManager.AddTorch(torch);
            myInventory.AddTorch(-1);
            torch.GetComponent<Torch>().SetVisible();
        }
    }     

    public void ApplyDamageToPlayer(int _amount, float _timeToChangeColor = .1f)
    {
        myStats.ApplyDamageToPlayer(_amount, _timeToChangeColor);
    }

    public void ApplyDotToPlayer(int _dotDamage, int _dotTimes, float _timeBwDamage)
    {
        myStats.SetPoison(_dotDamage, _dotTimes, _timeBwDamage);
    }

    public bool HasKey()
    {
        return myInventory.HasKey();
    }

    public void UseKey()
    {
        myInventory.UseKey();
    }

    public void TurnOnOffFlashLight()
    {
        flashLight.enabled = !flashLight.enabled;
    }

    public void ChangeSpeed(float _newSpeed, float _timeToResumeSpeed)
    {
        myStats.ChangeSpeed(_newSpeed, _timeToResumeSpeed);
    }

    public void ChangeColor(Color _newColor, float _timeToResumeColor)
    {
        myStats.ChangeColor(_newColor, _timeToResumeColor);
    }

    public void AddTorch(int _num)
    {
        myInventory.AddTorch(_num);
    }

    public void AddSnap(int _num)
    {
        myInventory.AddSnap(_num);
    }

    public void GetKey()
    {
        myInventory.GetKey();
    }
}
