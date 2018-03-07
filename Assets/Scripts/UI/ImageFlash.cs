// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 07/01/2018
// Description: Enables and disables UI Image for a flashing effect.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageFlash : MonoBehaviour
{
    [SerializeField]
    private float flashTime = 1f;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

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
            image.enabled = !image.enabled;
        }
    }
}