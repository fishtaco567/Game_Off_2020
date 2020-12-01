using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    public bool isInMenu;

    [SerializeField]
    public GameObject hud;

    // Use this for initialization
    void Start() {
        isInMenu = true;
    }

    // Update is called once per frame
    void Update() {

    }

    public void StartGame() {
        if(hud == null) {
            hud = GameObject.Find("UI").transform.Find("Canvas").Find("HUD").gameObject;
        }
        hud.SetActive(true);
        isInMenu = false;
    }

    public void OnMenu() {
        isInMenu = true;
    }

    public void OnCloseMenu() {
        isInMenu = false;
    }

}
