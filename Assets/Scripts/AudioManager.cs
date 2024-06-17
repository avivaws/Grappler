using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("======= Audio Source =======")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    [Header("======= Audio Clip =======")]
    public AudioClip ropeSent;
    public AudioClip ropeFail;
    public AudioClip ropeHook;
    public AudioClip ropeJump;
    public AudioClip click;
    
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
