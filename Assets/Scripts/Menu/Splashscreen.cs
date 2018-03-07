// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Continues to the main menu after a given time.

using UnityEngine;
using System.Collections;

public class Splashscreen : MonoBehaviour
{
    [SerializeField]
    private float displayTime = 6.4f;

    [SerializeField]
    private MenuControl menuControl;

    private void Awake()
    {
        Debug.Assert(menuControl, "Menu Control is NULL.");
        if (displayTime < 0f)
        {
            displayTime = 0f;
        }
    }

    private void OnEnable()
    {
        if (menuControl)
        {
            StartCoroutine(ContinueFromSplashscreen());
        }
    }

    private void Update()
    {
        if (isActiveAndEnabled && Input.anyKey)
        {
            StopAllCoroutines();
            ContiueToMainMenu();
        }
    }

    private IEnumerator ContinueFromSplashscreen()
    {
        yield return new WaitForSeconds(displayTime);
        if (menuControl)
        {
            ContiueToMainMenu();
        }
    }

    private void ContiueToMainMenu()
    {
        menuControl.ChangePanel(MenuPanelType.MainMenu);
    }
}