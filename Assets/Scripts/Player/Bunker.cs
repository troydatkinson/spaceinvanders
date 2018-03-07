// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Desctructable Bunkers.

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bunker : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int health = 8;

    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private float explosionLength = 0.4f;

    private SpriteRenderer spriteRenderer;
    private float alphaPerHealth = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(health > 0, "Health is less than or equal to zero.");
        Debug.Assert(explosion, "Explosion is NULL.");

        alphaPerHealth = 1f / health;
    }

    public void Damage(GameObject damageDealer)
    {
        health--;

        // Destruction.
        if (health <= 0)
        {
            Destroy();
            return;
        }

        // Sprite update.
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - alphaPerHealth);
    }

    private void Destroy()
    {
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

        gameObject.SetActive(false);
    }
}