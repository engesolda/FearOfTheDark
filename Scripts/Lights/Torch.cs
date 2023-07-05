using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.SceneManagement;

public class Torch : MonoBehaviour
{
    public Light torchLight;
     
    // Start is called before the first frame update
    void Start()
    {
        torchLight = Instantiate(torchLight, transform.position, transform.rotation);
        torchLight.GetComponent<MyLight>().SetOwner(gameObject);
        torchLight.transform.SetParent(transform);
        torchLight.transform.position = GetComponentInParent<Transform>().position;

        GameManager.GetInstance().Save_Event += Torch_Save_Event;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().Save_Event -= Torch_Save_Event;
    }

    private void Torch_Save_Event(object sender, string e)
    {
        XMLController.SaveObjectState(e, "Lights", "Torch", gameObject.GetInstanceID().ToString(), torchLight.GetComponent<MyLight>().GetAttributes());
    }

    public void SetLightAttributes(XmlAttributeCollection _attributes)
    {
        transform.position = Methods.ConvertStringToVector3(_attributes["position"].Value);
        torchLight.GetComponent<MyLight>().SetAttributes(_attributes);
    }

    public float GetLifeTime()
    {
        return torchLight.GetComponent<MyLight>().GetLifeTime();
    }

    public void SetLifeTime(float _lifetime)
    {
        torchLight.GetComponent<MyLight>().SetLifeTime(_lifetime);
    }

    public void SetInvisible()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        torchLight.GetComponent<MyLight>().enabled = false;
        torchLight.GetComponent<CircleCollider2D>().enabled = false;
    }

    public void SetVisible()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        torchLight.GetComponent<MyLight>().enabled = true;
        torchLight.GetComponent<CircleCollider2D>().enabled = true;
    }
}
