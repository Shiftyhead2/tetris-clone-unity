using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    public static SoundManager instance;
    public AudioClip[] SFX;
    AudioSource myAudioSource;

    
    void Start()
    {
        if(!instance)
        {
            instance = this;
        }

        myAudioSource = GetComponent<AudioSource>();
    }


    public void PlaySound(int whichSFX)
    {
        //This is just to avoid the annoying warning that the audio clip cannot be played because the audiosource is disabled
        if(myAudioSource.enabled || !myAudioSource.mute)
        {
            myAudioSource.PlayOneShot(SFX[whichSFX],myAudioSource.volume);
        }
    }

}
