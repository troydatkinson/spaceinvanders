// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Controls player movement, shooting and lives.

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float playerSpeed = 1.2f;

    [SerializeField, Tooltip("The player ship can only move inside the clamped area on the X axis.")]
    private float movementClamp = 2.6f;

    [SerializeField]
    private float weaponCooldown = 1f;

    [SerializeField]
    private float unlockControlsAfter = 0.5f;

    [SerializeField]
    private float respawnTime = 2.5f;

    [SerializeField]
    private GameObject playerMissle;

    [SerializeField]
    private AudioClip fireSound;

    [SerializeField]
    private float explosionLength = 0.4f;

    [SerializeField]
    private GameObject explosion;

    private bool alive = false;
    private Direction shipDirection;
    private bool canShoot = true;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Guards.
        Debug.Assert(playerSpeed > 0, "Player Speed is zero.");
        Debug.Assert(movementClamp > 0, "Movement Clamp is zero.");
        Debug.Assert(weaponCooldown > 0, "Weapon Cooldown is zero.");
        Debug.Assert(respawnTime > 0, "Respawn Time is zero.");
        Debug.Assert(unlockControlsAfter > 0, "Unlock Controls After is zero.");
        Debug.Assert(playerMissle, "Player Missle is NULL.");
        Debug.Assert(fireSound, "Fire Sound is NULL.");
        Debug.Assert(explosion, "Explosion is NULL.");
    }

    private void Start()
    {
        StartCoroutine(UnlockControls());
    }

    private void Update()
    {
        // Input is only active when the player is alive.
        if (!alive)
        {
            return;
        }
        MovementInput();
        WeaponInput();
    }

    private void FixedUpdate()
    {
        // Ship movement on screen.
        if (shipDirection == Direction.Left)
        {
            transform.Translate(Vector2.left * Time.deltaTime * playerSpeed);
        }
        else if (shipDirection == Direction.Right)
        {
            transform.Translate(Vector2.right * Time.deltaTime * playerSpeed);
        }
    }

    // Unlocks the controls after a small amount of time so the player can't do anything during the pan.
    private IEnumerator UnlockControls()
    {
        yield return new WaitForSeconds(unlockControlsAfter);
        alive = true;
    }

    private void MovementInput()
    {
        if (Input.GetButton("Left") && transform.position.x > -movementClamp)
        {
            shipDirection = Direction.Left;
        }

        else if (Input.GetButton("Right") && transform.position.x < movementClamp)
        {
            shipDirection = Direction.Right;
        }

        else
        {
            shipDirection = Direction.NoDirection;
        }
    }

    private void WeaponInput()
    {
        if (!canShoot)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        canShoot = false;

        if (playerMissle)
        {
            Instantiate(playerMissle, transform.position, Quaternion.identity);
        }

        if (fireSound)
        {
            audioSource.PlayOneShot(fireSound);
        }

        StartCoroutine(CooldownWeapon());
    }

    private IEnumerator CooldownWeapon()
    {
        yield return new WaitForSeconds(weaponCooldown);
        canShoot = true;

        // Repeated fire.
        if (Input.GetButton("Fire1"))
        {
            FireWeapon();
        }
    }

    public void Damage(GameObject damageDealer)
    {
        // Create and destroy explosion object.
        if (explosion)
        {
            GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(newExplosion, explosionLength);
        }

        // Remove life.
        alive = false;
        gameObject.SetActive(false);
        GameManager.Instance.StartCoroutine(Respawn());
        GameManager.Instance.PlayerDied();
    }

    private IEnumerator Respawn()
    {
        transform.position = new Vector2(0, transform.position.y);
        yield return new WaitForSeconds(respawnTime);

        gameObject.SetActive(true);
        alive = true;
        canShoot = true;
        shipDirection = Direction.NoDirection;
    }
}