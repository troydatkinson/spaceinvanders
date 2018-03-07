// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 07/01/2018
// Description: Enables and disables UI text for a flashing effect.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TextFlash : MonoBehaviour
{
    [SerializeField]
    private float flashTime = 1f;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        // Guard.
        if (flashTime <= 0f)
        {
            Debug.LogWarning("Flash time is too low.");
            return;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        while (true)
        {
            yield return new WaitForSeconds(flashTime);
            text.enabled = !text.enabled;
        }
    }
}