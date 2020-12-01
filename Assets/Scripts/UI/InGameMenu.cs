using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {

    [SerializeField]
    protected GameObject settingsMenu;

    [SerializeField]
    protected GameObject controlsMenu;

    [SerializeField]
    protected GameObject inGameMenu;

    public bool isOpen;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetupMenus() {
        inGameMenu.SetActive(false);
    }

    public void OpenInGameMenu() {
        GameManager.Instance.OnMenu();
        inGameMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 0;
        isOpen = true;
    }

    public void ResumePressed() {
        Debug.Log("Yes");
        GameManager.Instance.OnCloseMenu();
        inGameMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 1;
        isOpen = false;
    }

    public void QuitPressed() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SettingsPressed() {
        inGameMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<SubMenu>().lastMenu = this.gameObject;
    }

    public void ControlsPressed() {
        inGameMenu.SetActive(false);
        controlsMenu.SetActive(true);
        controlsMenu.GetComponent<SubMenu>().lastMenu = this.gameObject;
    }

    public void BackPressed() {
        inGameMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

}
