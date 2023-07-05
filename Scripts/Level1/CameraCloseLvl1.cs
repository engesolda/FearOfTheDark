using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCloseLvl1 : MonoBehaviour
{
    public float transformPosition;
    public float closeSpeed;

    private Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        endPos = new Vector3(-5.8f, 9.5f, -1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, endPos, transformPosition);
        transform.position = newPos;

        if (GetComponent<Camera>().orthographicSize > 0.1f)
        {
            GetComponent<Camera>().orthographicSize -= closeSpeed;
        }
        else
        {
            SceneManager.LoadScene("Level1IntroOutBed");
        }
    }
}
