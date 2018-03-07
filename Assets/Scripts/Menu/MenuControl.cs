// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Master script from menu switching.

using UnityEngine;

public class MenuControl : MonoBehaviour
{
    [SerializeField]
    private MenuPanelType startingPanel = MenuPanelType.Splashscreen;

    [SerializeField]
    private GameObject splashscreenPanel;

    [SerializeField]
    private GameObject mainMenuPanel;

    private void Awake()
    {
        Debug.Assert(splashscreenPanel, "Splashscreen Panel is NULL.");
        Debug.Assert(mainMenuPanel, "MainMenu Panel is NULL.");
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ChangePanel(startingPanel);
    }

    public void ChangePanel(MenuPanelType panel)
    {
        // Close all panels.
        mainMenuPanel.SetActive(false);
        splashscreenPanel.SetActive(false);

        // Open correct panel.
        switch (panel)
        {
            default:
            case MenuPanelType.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case MenuPanelType.Splashscreen:
                splashscreenPanel.SetActive(true);
                break;
        }
    }
}