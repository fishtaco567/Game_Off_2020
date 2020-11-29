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
    protected TMP_Text moneyText;
    [SerializeField]
    protected TMP_Text fuelText;

    [SerializeField]
    protected GameObject bumpNozzleIndicator;
    [SerializeField]
    protected GameObject restrictorNozzleIndicator;

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

    private Func<int, bool> callback;

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

        initialBumpPos = bumpNozzleIndicator.transform.localPosition;
        initialRestrictPos = restrictorNozzleIndicator.transform.localPosition;

        bumpNozzleIndicator.SetActive(false);
        restrictorNozzleIndicator.SetActive(false);
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
                        var b = callback?.Invoke(currentPosition);
                        if(b == false) {
                            isTextOn = false;
                            textMesh.enabled = false;
                            currentTime = 0;
                        }
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

        if(bumpCollected || restrictorCollected) {
            currentCollectTime += Time.deltaTime;

            if(currentCollectTime > collectDelay && currentCollectTime < collectTime + collectDelay) {
                var curFloatTime = currentCollectTime - collectDelay;
                if(bumpCollected) {
                    bumpNozzleIndicator.transform.localPosition = initialBumpPos * (curFloatTime / collectTime);
                } else if(restrictorCollected) {
                    restrictorNozzleIndicator.transform.localPosition = initialRestrictPos * (curFloatTime / collectTime);
                }
            } else if(currentCollectTime > collectTime + collectDelay) {
                if(bumpCollected) {
                    bumpNozzleIndicator.transform.localPosition = initialBumpPos;
                } else if(restrictorCollected) {
                    restrictorNozzleIndicator.transform.localPosition = initialRestrictPos;
                }
                bumpCollected = false;
                restrictorCollected = false;
            }
        }
    }

    public void StartText(string[] text, Func<int, bool> callback) {
        isTextOn = true;
        currentText = text;
        currentPosition = 0;
        currentTime = 0;

        this.callback = callback;

        callback?.Invoke(currentPosition);
    }

    public void UpdateMoneyText(int money) {
        moneyText.text = money.ToString();
    }

    public void UpdateFuelText(int fuel) {
        fuelText.text = fuel.ToString();
    }

    [SerializeField]
    public float collectTime;
    [SerializeField]
    public float collectDelay;

    [SerializeField]
    private float currentCollectTime;

    private Vector3 initialBumpPos;
    private Vector3 initialRestrictPos;

    private bool bumpCollected;
    private bool restrictorCollected;

    public void ObtainBump() {
        currentCollectTime = 0;
        bumpCollected = true;
        bumpNozzleIndicator.transform.localPosition = Vector3.zero;
        bumpNozzleIndicator.SetActive(true);
    }

    public void ObtainRestrictor() {
        currentCollectTime = 0;
        restrictorCollected = true;
        restrictorNozzleIndicator.transform.localPosition = Vector3.zero;
        restrictorNozzleIndicator.SetActive(true);
    }

}
