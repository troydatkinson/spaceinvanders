// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Scoring and round management.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private EnemyHorde enemyHorde;

    [SerializeField]
    private ScoreTable scoreTable = new ScoreTable();

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text highscoreText;

    [SerializeField]
    private Text livesText;

    [SerializeField]
    private GameObject[] livesIcons;

    [SerializeField]
    private Text endStatement;

    [SerializeField, Tooltip("How many seconds between waves loading in.")]
    private float waveLoadTime = 4f;

    // Singleton.
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } } 

    private int score = 0;
    private int lives = 3;
    private static int previousScore = 0;
    private static int previousLives = 0;
    private Highscore highscore = new Highscore();
    private int highscoreValue = 0;
    private bool beatenHighscore = false;

    private void Awake()
    {
        Debug.Assert(player, "Player is NULL.");
        Debug.Assert(enemyHorde, "Enemy Horde is NULL.");
        Debug.Assert(scoreText, "Score Text is NULL.");
        Debug.Assert(highscoreText, "Highscore Text is NULL.");
        Debug.Assert(livesText, "Lives Text is NULL.");
        Debug.Assert(endStatement, "End Statement is NULL.");
        Debug.Assert(livesIcons.Length == (lives - 1), "Lives Icons ism't equal to lives.");

        // Singleton setup.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Sort out persistent lives and score.
        livesText.text = lives.ToString();
        for (int i = 0; i < lives - previousLives; i++)
        {
            PlayerDied();
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        // Loads score from last wave.
        AddScore(previousScore);

        // Get highscore.
        highscoreValue = highscore.GetHighscore();
        highscoreText.text = highscoreValue.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public static void NewGame()
    {
        previousScore = 0;
        previousLives = 3;
        EnemyHorde.ResetWaves();
    }

    public void EnemyKilled(Enemy enemy, EnemyType enemyType)
    {
        AddScore(scoreTable.Score(enemyType));
        enemyHorde.EnemyKilled(enemy);
    }

    private void AddScore(int scoreToAdd)
    {
        // Guard.
        if (scoreToAdd <= 0 )
        {
            return;
        }

        score += scoreToAdd;
        scoreText.text = score.ToString();

        // Highscore.
        if (score > highscoreValue)
        {
            highscoreText.text = score.ToString();

            if (!beatenHighscore)
            {
                // Add a flash when the player beats their highscore.
                highscoreText.gameObject.AddComponent<TextFlash>();
                beatenHighscore = true;
            }
        }    
    }

    public void WaveComplete()
    {
        previousScore = score;
        previousLives = lives;
        endStatement.text = "WAVE COMPLETE";
        endStatement.gameObject.SetActive(true);
        EnemyHorde.CompleteWave();
        StartCoroutine(NextWave());
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(waveLoadTime);
        SceneManager.LoadScene(1);
    }

    public void PlayerDied()
    {
        lives--;
        livesText.text = lives.ToString();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // Update UI ships.
            if (livesIcons.Length >= (lives))
            {
                livesIcons[lives - 1].SetActive(false);
            }
        }
    }

    public void LoseGame()
    {
        GameOver();
    }

    private void GameOver()
    {
        StopAllCoroutines();
        endStatement.text = "GAME OVER";
        endStatement.gameObject.SetActive(true);
        Destroy(enemyHorde.gameObject);
        Destroy(player.gameObject);

        // Highscore.
        if (score > highscoreValue)
        {
            highscore.SetHighscore(score);
        }
    }
}