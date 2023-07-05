using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    //Public
    public List<AudioClip> audioClips;

    //Private
    private float startPitch;
    private float startVolume;
    private AudioSource hitGruntSound;
    private AudioSource footStepSound;
    private AudioSource snapThrowSound;
    private AudioSource healSound;

    // Start is called before the first frame update
    void Start()
    {
        hitGruntSound = gameObject.AddComponent<AudioSource>();
        footStepSound = gameObject.AddComponent<AudioSource>();
        snapThrowSound = gameObject.AddComponent<AudioSource>();
        healSound = gameObject.AddComponent<AudioSource>();
        hitGruntSound.clip = audioClips.Find(clip => clip.name.Equals("Grunt"));
        footStepSound.clip = audioClips.Find(clip => clip.name.Equals("FootstepWood"));
        snapThrowSound.clip = audioClips.Find(clip => clip.name.Equals("SnapThrow"));
        healSound.clip = audioClips.Find(clip => clip.name.Equals("Heal"));

        healSound.volume = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHitGruntAudio()
    {
        //Play the sound
        hitGruntSound.volume = Random.Range(.8f, 1);
        hitGruntSound.pitch = Random.Range(.8f, 1.2f);
        hitGruntSound.Play();
    }

    public void PlayHealAudio()
    {
        //Play the sound
        healSound.pitch = Random.Range(.8f, 1.2f);
        healSound.Play();
    }

    public void PlayWalkOnWood(bool _stopped)
    {
        if (_stopped)
        {
            footStepSound.Stop();
            return;
        }

        if (!footStepSound.isPlaying)
        {
            //Save the initial values
            startPitch = footStepSound.pitch;
            startVolume = footStepSound.volume;

            //Play the sound
            footStepSound.pitch = Random.Range(startPitch - 0.2f, startPitch + 0.2f);
            footStepSound.volume = Random.Range(startVolume - 0.2f, 1f);
            footStepSound.Play();

            //Restore the initial values
            footStepSound.pitch = startPitch;
            footStepSound.volume = startVolume;
        }
    }

    public void PlayThrowingSnapSound()
    {
        //Save the initial values
        startPitch = snapThrowSound.pitch;
        startVolume = snapThrowSound.volume;

        //Play the sound
        snapThrowSound.pitch = Random.Range(startPitch - 0.2f, startPitch + 0.2f);
        snapThrowSound.volume = Random.Range(startVolume - 0.2f, 1f);
        snapThrowSound.Play();

        //Restore the initial values
        snapThrowSound.pitch = startPitch;
        snapThrowSound.volume = startVolume;
    }
}
