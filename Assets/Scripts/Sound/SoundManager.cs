using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    static public SoundManager instance;

    [Tooltip("音效：捡金币")]
    public AudioClip pickCoin;
    [Tooltip("音效：投金币")]
    public AudioClip throwCoin;

    private AudioSource audioSource;

    void Start() {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickCoin() {
        if (pickCoin == null) return;
        audioSource.PlayOneShot(pickCoin);
    }

    public void PlayThrowCoin() {
        if (pickCoin == null) return;
        audioSource.PlayOneShot(throwCoin);
    }
}
