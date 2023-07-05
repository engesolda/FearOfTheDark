using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryController : MonoBehaviour
{
    //Public
    public GameObject gameMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDiaryClick()
    {
        Time.timeScale = 0;
        gameMenu.SetActive(true);
    }
}
