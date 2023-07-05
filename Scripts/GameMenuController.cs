using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    //Public
    public int index;
    public int numMenus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            if (Input.GetKeyDown("up"))
            {
                index--;

                if (index < 0)
                {
                    index = numMenus - 1;
                }
            }
            else if (Input.GetKeyDown("down"))
            {
                index++;

                if (index > numMenus - 1)
                {
                    index = 0;
                }
            }
        }
    }
}
