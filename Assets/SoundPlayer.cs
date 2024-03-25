using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    public void SetupSound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        Invoke(nameof(DestroyAfterPlay), clip.length);
    }
    
    public void SetupSound(AudioClip clip, float pitch)
    {
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.Play();
        Invoke(nameof(DestroyAfterPlay), clip.length);
    }
    
    void DestroyAfterPlay()
    {
        Destroy(gameObject);
    }


}
