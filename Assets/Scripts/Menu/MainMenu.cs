// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Master script from menu switching.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private AudioClip confirmSound;

    [SerializeField]
    private AudioClip quitSound;

    [SerializeField]
    private Text highscoreValue;

    [SerializeField]
    private Button firstSelected;

    private const string gameSceneName = "Game";

    private void Awake()
    {
        Debug.Assert(confirmSound, "Confirm Sound is NULL.");
        Debug.Assert(quitSound, "Quit Sound Sound is NULL.");
        Debug.Assert(highscoreValue, "Highscore Value is NULL.");
        Debug.Assert(firstSelected, "First Selected Value is NULL.");
    }

    private void Start()
    {
        // Set highscore.
        if (highscoreValue)
        {
            highscoreValue.text = new Highscore().GetHighscore().ToString();
        }
    }

    private void OnEnable()
    {
        // Highlight first option.
        if (firstSelected)
        {
            firstSelected.Select();
            firstSelected.OnSelect(null);
        }
    }

    public void ContinueToGame()
    {
        SceneManager.LoadScene(gameSceneName);
        GameManager.NewGame();

        if (confirmSound)
        {
            Music.PlayerOneShot(confirmSound);
        }
    }

    public void ExitGame()
    {
        Application.Quit();

        if (quitSound)
        {
            Music.PlayerOneShot(quitSound);
        }
    }
}
