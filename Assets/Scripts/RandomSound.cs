using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    [SerializeField] AudioClip[] clips;
    [SerializeField] float pitchShift = 0.2f;
    [SerializeField] AudioSource source;

    private void Awake()
    {
        source.clip = clips[Random.Range(0, clips.Length)];
        source.pitch += Random.Range(-pitchShift, pitchShift);
        source.Play();
    }
}
