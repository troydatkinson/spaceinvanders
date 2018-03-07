// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 07/01/2018
// Description: Saves and loads highscore from Playerprefs, uses a checksum to ensure no outside changes were made to the highscore.

using UnityEngine;
using System.Text;
using System.Security.Cryptography;

public class Highscore
{
    private int highscore = 0;

    private const string highscoreKey = "Highscore";
    private const string checksumKey = "Checksum";
    private const string salt = "0052c8ef7d749ff8cd1bac9f6af071b9";

    MD5 md5 = new MD5CryptoServiceProvider();

    public void SetHighscore(int score)
    {
        // Ensure actual highscore.
        if (score <= highscore)
        {
            Debug.LogWarning("Score is less than or equal to player's highscore.");
            return;
        }

        // Generate checksum for score.
        byte[] byteHash = md5.ComputeHash(Encoding.UTF8.GetBytes(salt + score));
        string hash = Encoding.UTF8.GetString(byteHash);

        // Save values.
        PlayerPrefs.SetInt(highscoreKey, score);
        PlayerPrefs.SetString(checksumKey, hash);
    }

    public int GetHighscore()
    {
        // Ensure values exist.
        if (!PlayerPrefs.HasKey(highscoreKey) || !PlayerPrefs.HasKey(checksumKey))
        {
            return 0;
        }

        // Get values.
        int score = PlayerPrefs.GetInt("Highscore");
        string checksum = PlayerPrefs.GetString("Checksum");

        // Check values.
        if (score <= 0 || string.IsNullOrEmpty(checksum))
        {
            return 0;
        }

        // Generate checksum with highscore from prefs.
        byte[] byteHash = md5.ComputeHash(Encoding.UTF8.GetBytes(salt + score));
        string hash = Encoding.UTF8.GetString(byteHash);

        // Compare values.
        if (hash != checksum)
        {
            Debug.LogError("Detected that the highsore has been edited out of program.");
            return 0;
        }

        highscore = score;
        return score;
    }
}