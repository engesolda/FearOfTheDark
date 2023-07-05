using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatsController : MonoBehaviour
{
    //Public
    public int maxHitpoints;
    public int currentHitpoints;
    public bool beignDragged = false;
    public float playerSpeed;

    //Private
    private int poisonsLeft = 0;
    private int poisonDamage = 0;
    private float poisonTimeBwDamages = 0;
    private float defaultSpeed = 0;
    private SpriteRenderer myRederer;
    private SoundController mySounds;
    private Color defaultColor;
    private Color currentColor;
    private Color normalHealColor = Color.green;
    private Color damageColor = Color.red;
    private Color normalDamageColor = Color.red;
    private Color posionColor = new Color32(115, 50, 115, 1);
    private Text hpText;

    private void Start()
    {
        mySounds = GetComponent<SoundController>();
        myRederer = GetComponent<SpriteRenderer>();
        defaultSpeed = playerSpeed;        
        defaultColor = myRederer.color;
        currentColor = defaultColor;
        currentHitpoints = maxHitpoints;

        hpText = GameObject.Find("Hitpoints").GetComponent<Text>();
    }

    public void Initialize(StatsController _loadedStats)
    {
        hpText = GameObject.Find("Hitpoints").GetComponent<Text>();

        if (_loadedStats != null)
        {
            playerSpeed = _loadedStats.playerSpeed;
            currentHitpoints = _loadedStats.currentHitpoints;
        }
        else
        {
            currentHitpoints = maxHitpoints;
            playerSpeed = defaultSpeed;
        }

        hpText.text = "Hitpoints: " + currentHitpoints;
    }

    //-----HP Manipulation-----//
    public void ApplyDamageToPlayer(int _amount, float _timeToChangeColor = .1f)
    {
        currentHitpoints += _amount;

        if (currentHitpoints < 0)
        {
            currentHitpoints = 0;
        }
        if (currentHitpoints > maxHitpoints)
        {
            currentHitpoints = maxHitpoints;
        }

        if (_amount > 0)
        {
            mySounds.PlayHealAudio();
            ChangeColor(normalHealColor, _timeToChangeColor);
            FloatingTextController.CreateFloatingText(Camera.main.WorldToScreenPoint(transform.position) + new Vector3(-5, 40, 0), '+' + _amount.ToString(), normalHealColor, 35);
        }
        else
        {
            mySounds.PlayHitGruntAudio();
            ChangeColor(damageColor, _timeToChangeColor);
            FloatingTextController.CreateFloatingText(Camera.main.WorldToScreenPoint(transform.position) + new Vector3(-5, 40, 0), _amount.ToString(), damageColor, 35);
        }

        if (hpText != null)
        {
            //Display Player Life
            if (currentHitpoints > 0)
            {
                hpText.text = "Hitpoints: " + currentHitpoints;
            }
            else
            {
                hpText.text = "Hitpoints: " + currentHitpoints + " you died =/";
            }
        }
    }

    public void SetPoison(int _dotDamage, int _dotTimes, float _timeBwDamage)
    {
        poisonTimeBwDamages = _timeBwDamage;
        poisonsLeft = _dotTimes;
        poisonDamage = _dotDamage;
        InvokeRepeating("ApplyPoisonDamage", poisonTimeBwDamages, poisonTimeBwDamages);
    }

    private void ApplyPoisonDamage()
    {
        if (poisonsLeft > 0)
        {
            damageColor = posionColor;
            ApplyDamageToPlayer(poisonDamage);
            damageColor = normalDamageColor;

            poisonsLeft--;
        }

        if (poisonsLeft <= 0)
        {
            CancelInvoke("ApplyPoisonDamage");
        }
    }

    //-----Speed Manipulation-----//
    public void ChangeSpeed(float _newSpeed, float _timeToResumeSpeed)
    {
        playerSpeed += _newSpeed;
        Invoke("ResumeSpeed", _timeToResumeSpeed);
    }

    private void ResumeSpeed()
    {
        playerSpeed = defaultSpeed;
    }

    //-----Color Manipulation-----//
    public void ChangeColor(Color _newColor, float _timeToResume)
    {
        currentColor = myRederer.color;
        _newColor.a = 1;
        myRederer.color = _newColor;

        //Sometimes my current color will be different from the default like when Im slowed down and the I receive a hit.
        if (currentColor != defaultColor)
        {
            Invoke("ResumeColorToLastState", _timeToResume);
        }
        else
        {
            Invoke("ResumeColorToDefault", _timeToResume);
        }        
    }

    private void ResumeColorToLastState()
    {
        myRederer.color = currentColor;
    }

    private void ResumeColorToDefault()
    {
        myRederer.color = defaultColor;
    }

    //-----Save & Load-----//
    public List<XMLController.XmlNodeAttributes> SaveStats()
    {
        List<XMLController.XmlNodeAttributes> stats = new List<XMLController.XmlNodeAttributes>();

        stats.Add(new XMLController.XmlNodeAttributes("id", "stats"));
        stats.Add(new XMLController.XmlNodeAttributes("maxHitpoints", maxHitpoints.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("currentHitpoints", currentHitpoints.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("beignDragged", Methods.ConvertBoolToString(beignDragged)));
        stats.Add(new XMLController.XmlNodeAttributes("playerSpeed", playerSpeed.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("poisonsLeft", poisonsLeft.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("poisonDamage", poisonDamage.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("poisonTimeBwDamages", poisonTimeBwDamages.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("defaultSpeed", defaultSpeed.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("defaultColor", defaultColor.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("currentColor", currentColor.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("normalHealColor", normalHealColor.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("damageColor", damageColor.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("normalDamageColor", normalDamageColor.ToString()));
        stats.Add(new XMLController.XmlNodeAttributes("posionColor", posionColor.ToString()));

        return stats;
    }

    public void LoadStats(List<XMLController.XmlNodeAttributes> _attributes)
    {
        foreach (XMLController.XmlNodeAttributes attribute in _attributes)
        {
            if (attribute.title.Equals("maxHitpoints"))
            {
                maxHitpoints = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("currentHitpoints"))
            {
                currentHitpoints = int.Parse(attribute.value);
                hpText.text = "Hitpoints: " + currentHitpoints;
            }
            if (attribute.title.Equals("beignDragged"))
            {
                beignDragged = Methods.ConvertStringToBool(attribute.value);
            }
            if (attribute.title.Equals("playerSpeed"))
            {
                playerSpeed = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("poisonsLeft"))
            {
                poisonsLeft = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("poisonDamage"))
            {
                poisonDamage = int.Parse(attribute.value);
            }
            if (attribute.title.Equals("poisonTimeBwDamages"))
            {
                poisonTimeBwDamages = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("defaultSpeed"))
            {
                defaultSpeed = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("defaultColor"))
            {
                //defaultColor = attribute.value;
            }
            if (attribute.title.Equals("currentColor"))
            {
                //currentColor = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("normalHealColor"))
            {
                //normalHealColor = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("damageColor"))
            {
                //damageColor = float.Parse(attribute.value);
            }
            if (attribute.title.Equals("posionColor"))
            {
                //posionColor = float.Parse(attribute.value);
            }
        }
    }
}
