// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 07/01/2018
// Description: Decrease fill amount on Filled Image component.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageSwipe : MonoBehaviour
{
    [SerializeField]
    private float swipeSpeed = 0.25f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        image.fillAmount = 1f;
        StartCoroutine(Swipe());
    }

    private IEnumerator Swipe()
    {
        while (image.fillAmount > 0)
        {
            image.fillAmount -= Time.deltaTime * swipeSpeed;
            yield return null;
        }
    }
}