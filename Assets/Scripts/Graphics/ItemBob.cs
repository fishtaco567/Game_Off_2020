using UnityEngine;
using System.Collections;

public class ItemBob : MonoBehaviour {

    [SerializeField]
    private float bobTime;

    [SerializeField]
    private float bobAmount;

    [SerializeField]
    private float rotateRate;

    private Vector3 basePos;
    private Quaternion baseRot;

    // Use this for initialization
    void Start() {
        baseRot = transform.localRotation;
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        var currentBob = Mathf.Sin(Time.time / bobTime) * bobAmount;

        var currentRotate = (Time.time / rotateRate) * 360f;

        transform.position = basePos + transform.up * currentBob;

        transform.localRotation = baseRot * Quaternion.AngleAxis(currentRotate, Vector3.up);
    }
}
