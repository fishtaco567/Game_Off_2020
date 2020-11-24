using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Rewired;
using TMPro;

public class UIManager : Singleton<UIManager> {

    [SerializeField]
    protected GameObject textPane;
    protected TMP_Text textMesh;
    protected RectTransform textBackground;

    [SerializeField]
    protected Vector2 panelMinSize;
    
    [SerializeField]
    protected Vector2 panelMaxSize;

    [SerializeField]
    protected AnimationCurve openCurve;

    [SerializeField]
    protected AnimationCurve closeCurve;

    [SerializeField]
    protected float openTime;
    [SerializeField]
    protected float closeTime;

    public bool isTextOn;
    private Player playerInput;

    public bool focused;
    
    private string[] currentText;
    private int currentPosition;

    [SerializeField]
    private float currentTime;

    private Action<int> callback;

    protected void Start() {
        textMesh = textPane.GetComponentInChildren<TMP_Text>();
        textBackground = textPane.GetComponent<RectTransform>();

        isTextOn = false;
        playerInput = ReInput.players.GetPlayer(0);

        openTime = openCurve.keys[openCurve.keys.Length - 1].time;
        closeTime = closeCurve.keys[closeCurve.keys.Length - 1].time;

        currentTime = 100;

        textMesh.enabled = false;
        textPane.SetActive(false);

        focused = false;
    }

    protected void Update() {
        bool selectPressed = playerInput.GetButtonDown("Select");
        currentTime += Time.unscaledDeltaTime;

        if(isTextOn) {
            focused = true;

            if(currentTime < openTime) {
                textPane.SetActive(true);
                textBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelMinSize.x + (panelMaxSize.x - panelMinSize.x) * openCurve.Evaluate(currentTime));
                textBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelMinSize.y + (panelMaxSize.y - panelMinSize.y) * openCurve.Evaluate(currentTime));
            } else {
                textMesh.enabled = true;
                textMesh.text = currentText[currentPosition];

                if(selectPressed) {
                    currentPosition++;

                    if(currentPosition >= currentText.Length) {
                        isTextOn = false;
                        textMesh.enabled = false;
                        currentTime = 0;
                    } else {
                        callback?.Invoke(currentPosition);
                    }
                }
            }
        } else {
            if(currentTime < closeTime) {
                textBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelMinSize.x + (panelMaxSize.x - panelMinSize.x) * closeCurve.Evaluate(currentTime));
                textBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelMinSize.y + (panelMaxSize.y - panelMinSize.y) * closeCurve.Evaluate(currentTime));
            } else {
                focused = false;
                textPane.SetActive(false);
            }
        }
    }

    public void StartText(string[] text, Action<int> callback) {
        isTextOn = true;
        currentText = text;
        currentPosition = 0;
        currentTime = 0;

        this.callback = callback;

        callback?.Invoke(currentPosition);
    }

}
