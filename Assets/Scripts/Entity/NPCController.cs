﻿using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour {

    public Action<PlayerController, NPCController> OnInteract;

    [SerializeField]
    protected AudioClip interactAudio;

    [SerializeField]
    protected float minPitch = 1;

    [SerializeField]
    protected float maxPitch = 1;

    [SerializeField]
    protected bool hasAnimator;

    protected Animator animator;

    protected AudioSource interactAudioSource;

    protected void Start() {
        if(interactAudio != null) {
            interactAudioSource = gameObject.AddComponent<AudioSource>();
            interactAudioSource.clip = interactAudio;
        }

        if(hasAnimator) {
            animator = GetComponent<Animator>();
        }
    }

    protected void Update() {

    }

    public void Interact(PlayerController player) {
        if(hasAnimator) {
            animator.SetTrigger("Interact");
        }

        if(interactAudioSource != null) {
            float pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            interactAudioSource.pitch = pitch;
            interactAudioSource.Play();
        }

        OnInteract?.Invoke(player, this);
    }

}