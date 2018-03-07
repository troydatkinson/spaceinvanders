// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Floating enemy aliens, shoot and death functionalility.

using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private EnemyType enemyType = EnemyType.Weak;

    [SerializeField]
    private Sprite alternateSprite;

    [SerializeField]
    private float explosionLength = 0.4f;

    [SerializeField]
    private GameObject explosion;

    private SpriteRenderer spriteRenderer;
    private bool moveSpriteThisTick = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Assert(alternateSprite, "Alternate Sprite is NULL.");
        Debug.Assert(explosion, "Explosion is NULL.");
    }

    public void FlipSprite()
    {
        // Guard.
        if (alternateSprite == null)
        {
            return;
        }

        // Added to slow down sprite swapping.
        if (!moveSpriteThisTick)
        {
            moveSpriteThisTick = true;
            return;
        }

        Sprite newSprite = alternateSprite;
        alternateSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = newSprite;
        moveSpriteThisTick = false;
    }

    public void Damage(GameObject damageDealer)
    {
        // Create and destroy explosion object.
        if (explosion)
        {
            GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);

            // Set explosion color.
            SpriteRenderer explosionSprite = newExplosion.GetComponent<SpriteRenderer>();
            if (explosionSprite)
            {
                explosionSprite.color = spriteRenderer.color;
            }

            Destroy(newExplosion, explosionLength);
        }

        // Add score and destroy.
        GameManager.Instance.EnemyKilled(this, enemyType);
        Destroy(gameObject);
    }
}