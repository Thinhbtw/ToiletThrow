using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffSound : MonoBehaviour
{
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (audioSource.loop) return;
        if (audioSource.isPlaying) return;
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
