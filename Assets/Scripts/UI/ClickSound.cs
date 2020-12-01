using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public Button button;

    public AudioSource downSource;
    public AudioSource upSource;

    public void OnPointerDown(PointerEventData eventData) {
        if(button.IsInteractable()) {
            downSource.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(button.IsInteractable()) {
            upSource.Play();
        }
    }

    // Use this for initialization
    void Start() {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
