using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;

public class MyLight : MonoBehaviour
{
    //Public
    public float lightIntensity;
    public float maxRange;
    public float lifeTime;
    public float lightRange;

    //Private
    private float iniLifetime;
    private float randomStop;
    private bool midLife = false;
    private GameObject owner;    
    private Light myLight;
    
    void Start()
    {
        iniLifetime = lifeTime;

        //Configuring the light
        myLight = GetComponent<Light>();
        myLight.intensity = lightIntensity;
        myLight.range = lightRange;
        randomStop = UnityEngine.Random.Range(0f, maxRange);
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        GetComponent<CircleCollider2D>().radius = myLight.range/2;

        if (maxRange > 0)
        {
            InvokeRepeating("PulseLight", 0, .2f);
        }
    }

    void Update()
    {
        if (owner && owner.CompareTag("Player"))
        {
            FollowPlayer();
        }

        if (lifeTime <= iniLifetime / 2 && !midLife)
        {//Light will be reduce in half at mid life

            lightIntensity = lightIntensity / 2;
            myLight.range = myLight.range / 2;
            maxRange = maxRange <= 1 ? 1 : maxRange / 2;
            myLight.intensity = lightIntensity;
            GetComponent<CircleCollider2D>().radius = myLight.range/2;
            midLife = true;
        }
    }

    public void SetAttributes(XmlAttributeCollection _attributes)
    {
        if (_attributes != null)
        {
            foreach (XmlAttribute attribute in _attributes)
            {
                if (attribute.Name.Equals("position"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, -1);
                }
                else if (attribute.Name.Equals("midLife"))
                {
                    midLife = Methods.ConvertStringToBool(attribute.Value);
                }
                else if (attribute.Name.Equals("lifeTime"))
                {
                    lifeTime = float.Parse(attribute.Value);
                }
                else if (attribute.Name.Equals("lightIntensity"))
                {
                    lightIntensity = float.Parse(attribute.Value);
                }
                else if (attribute.Name.Equals("lightRange"))
                {
                    lightRange = float.Parse(attribute.Value);
                }
                else if (attribute.Name.Equals("maxRange"))
                {
                    maxRange = float.Parse(attribute.Value);
                }
            }
        }
    }

    public List<XMLController.XmlNodeAttributes> GetAttributes()
    {
        List<XMLController.XmlNodeAttributes> attributes = new List<XMLController.XmlNodeAttributes>();

        attributes.Add(new XMLController.XmlNodeAttributes("scene", SceneManager.GetActiveScene().name));
        attributes.Add(new XMLController.XmlNodeAttributes("position", transform.parent.position.ToString()));
        attributes.Add(new XMLController.XmlNodeAttributes("midLife", Methods.ConvertBoolToString(midLife)));
        attributes.Add(new XMLController.XmlNodeAttributes("lifeTime", lifeTime.ToString()));
        attributes.Add(new XMLController.XmlNodeAttributes("lightIntensity", lightIntensity.ToString()));
        attributes.Add(new XMLController.XmlNodeAttributes("lightRange", lightRange.ToString()));
        attributes.Add(new XMLController.XmlNodeAttributes("maxRange", maxRange.ToString()));
        attributes.Add(new XMLController.XmlNodeAttributes("maxRange", maxRange.ToString()));

        return attributes;
    }

    public void SetOwner(GameObject _owner)
    {
        owner = _owner;
    }

    //Creates the light pulse effect
    void PulseLight()
    {
        float lightInt = myLight.intensity;
        if (lightInt + .5f <= lightIntensity + randomStop)
        {
            myLight.intensity = lightInt + .5f;
            randomStop = UnityEngine.Random.Range(0, maxRange);
        }
        else
        {
            myLight.intensity = lightInt - .5f;
        }
    }

    void FollowPlayer()
    {
        Vector3 lightPos = owner.transform.position;
        lightPos.z = -1;
        transform.position = lightPos;
    }

    public float GetLifeTime()
    {
        return lifeTime;
    }

    public void SetLifeTime(float _lifetime)
    {
        lifeTime -= _lifetime;
    }
}