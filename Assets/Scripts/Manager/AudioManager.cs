using System.Collections.Generic;
using UnityEngine;

public class AudioManager : UnitySingleton<AudioManager>
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.transform.position = position;
        audioSource.Play();
    }

}
