using UnityEngine;
using System.Collections;
using System;

public class NPCController : MonoBehaviour {

    public Action<PlayerController, NPCController> OnInteract;
    public Func<int, NPCController, bool> OnDisplayBox;

    public Predicate<NPCController> PlaySecond;

    [SerializeField]
    protected float minPitch = 1;

    [SerializeField]
    protected float maxPitch = 1;

    [SerializeField]
    protected bool hasAnimator;

    protected Animator animator;

    [SerializeField]
    protected AudioSource interactAudioSource;

    [SerializeField]
    protected string[] text;

    [SerializeField]
    protected string[] text2;

    protected void Start() {
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

        var playSecond = PlaySecond?.Invoke(this);

        if(playSecond.HasValue && playSecond.Value) {
            UIManager.Instance.StartText(text2, OnTextbox);
        } else {
            UIManager.Instance.StartText(text, OnTextbox);
        }

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
