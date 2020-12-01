using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    protected GameObject settingsMenu;

    [SerializeField]
    protected GameObject controlsMenu;

    [SerializeField]
    protected GameObject mainMenu;

    [SerializeField]
    protected GameObject titleImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMenus() {
        mainMenu.SetActive(true);
        titleImage.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void ExitPressed() {
        Application.Quit();
    }

    public void SettingsPressed() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenu.GetComponent<SubMenu>().lastMenu = this.gameObject;
    }

    public void ControlsPressed() {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
        controlsMenu.GetComponent<SubMenu>().lastMenu = this.gameObject;
    }

    public void PlayPressed() {
        mainMenu.SetActive(false);
        titleImage.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void BackPressed() {
        if(settingsMenu.activeInHierarchy) {
            settingsMenu.SetActive(false);
            settingsMenu.GetComponent<SubMenu>().lastMenu.SetActive(true);
        }

        if(controlsMenu.activeInHierarchy) {
            controlsMenu.SetActive(false);
            controlsMenu.GetComponent<SubMenu>().lastMenu.SetActive(true);
        }
    }

}
