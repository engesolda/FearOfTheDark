using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSound : MonoBehaviour
{
    private AudioSource myAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    private void PlaySound1()
    {
        myAudio.pitch = 1.2f;
        myAudio.Play();
    }

    private void PlaySound2()
    {
        myAudio.pitch = 1f;
        myAudio.Play();
    }
}
