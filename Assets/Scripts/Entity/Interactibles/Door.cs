using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    private Vector3 basePos;

    [SerializeField]
    float timeToGo;

    [SerializeField]
    float distanceToGo;

    [SerializeField]
    Transform localTransform;

    [SerializeField]
    GameObject otherDoor;

    bool isOpen;
    float currentTime;

    // Use this for initialization
    void Start() {
        basePos = transform.position;
        isOpen = false;
        currentTime = 100;
    }

    // Update is called once per frame
    void Update() {
        currentTime += Time.deltaTime;

        if(isOpen && currentTime < timeToGo) {
            transform.position = basePos - localTransform.up * (distanceToGo / timeToGo) * currentTime;
        } else if(!isOpen && currentTime < timeToGo) {
            transform.position = basePos - localTransform.up * distanceToGo + localTransform.up * (distanceToGo / timeToGo) * currentTime;
        }

        otherDoor.transform.position = transform.position;
    }

    public void SetOpen(bool open) {
        isOpen = open;
        currentTime = 0;
    }

}
