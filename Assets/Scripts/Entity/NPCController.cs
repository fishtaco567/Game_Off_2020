using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour {

    public Action<PlayerController, NPCController> OnInteract;
    public Func<int, NPCController, bool> OnDisplayBox;

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

    [SerializeField]
    protected string[] text;


    protected void Start() {
        if(interactAudio != null) {
            interactAudioSource = gameObject.AddComponent<AudioSource>();
            interactAudioSource.clip = interactAudio;
        }

        if(hasAnimator) {
            animator = GetComponentInChildren<Animator>();
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

        UIManager.Instance.StartText(text, OnTextbox);
    }

    public bool OnTextbox(int i) {
        var b = OnDisplayBox?.Invoke(i, this);
        if(b.HasValue) {
            return b.Value;
        } else {
            return true;
        }
    }

}
