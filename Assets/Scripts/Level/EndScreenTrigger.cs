using UnityEngine;
using System.Collections;

public class EndScreenTrigger : MonoBehaviour {

    [SerializeField]
    GameObject endMenu;

    bool entered;

    // Use this for initialization
    void Start() {
        entered = false;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<PlayerController>() && !entered) {
            entered = true;
            endMenu.SetActive(true);
            GameManager.Instance.OnMenu();
        }
    }
}
