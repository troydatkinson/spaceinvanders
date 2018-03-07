// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Missle that is fired from the player or enemies, travels and damages things.

using UnityEngine;

public class Missle : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Direction flightDirection = Direction.Up;

    [SerializeField]
    private float flySpeed = 3f;

    [SerializeField]
    private float selfDestructionTime = 10f;

    private void Awake()
    {
        Debug.Assert(flightDirection == Direction.Up || flightDirection == Direction.Down, "Flight Direction isn't up or down.");
        Debug.Assert(flySpeed > 0, "Fly Speed is zero.");
        Debug.Assert(selfDestructionTime > 0, "Self Destruction Time is zero.");
    }

    private void Start()
    {
        Destroy(gameObject, selfDestructionTime);
    }

    private void FixedUpdate()
    {
        transform.Translate((flightDirection == Direction.Down ? Vector2.down : Vector2.up) * Time.deltaTime * flySpeed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Damage anything that it hits.
        IDamageable other = collider.gameObject.GetComponent<IDamageable>();
        if (other != null)
        {
            other.Damage(gameObject);
        }

        Destroy(gameObject);
    }

    public void Damage(GameObject damageDealer)
    {
        // Inflict damage if damaged itself.
        IDamageable other = damageDealer.GetComponent<IDamageable>();
        if (other != null)
        {
            other.Damage(gameObject);
        }

        Destroy(gameObject);
    }
}