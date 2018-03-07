// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Controls the horde of enemies, movement and waves.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyHorde : MonoBehaviour
{
    [SerializeField, Tooltip("The delay between each enemy tick, decreasing will increase enemy speed.")]
    private float tickSpeed = 1f;

    [SerializeField, Tooltip("How much faster the ticks are every time an enemy is killed.")]
    private float tickDecreasePerEnemyDeath = 0.07f;

    [SerializeField, Tooltip("Delay between enemies reacting to a tick.")]
    private float spriteDelay = 0.05f;

    [SerializeField, Tooltip("Delay between enemy rows reacting to a tick.")]
    private float rowDelay = 0.18f;

    [SerializeField]
    private float horizontalMoveAmount = 0.1f;

    [SerializeField]
    private float verticalMoveAmount = 0.2f;

    [SerializeField, Tooltip("Player will lose if an invader gets to this depth.")]
    private float loseAtYAxis = -0.8f;

    [SerializeField, Tooltip("How far in each X direction before switching direction.")]
    private float movementClamp = 2.6f;

    [SerializeField]
    private Text waveText;

    [SerializeField]
    private GameObject enemyMissle;

    [SerializeField]
    private float minShootSpeed = 2f;

    [SerializeField]
    private float maxShootSpeed = 4f;

    // Doubles up as Level.
    private static int wave = 0;

    // The horde and the direction of their movement.
    private List<List<Enemy>> enemies = new List<List<Enemy>>();
    private int numberOfEnemies = 0;
    private Direction movementDirection = Direction.Left;
    private const float minTickSpeed = 0.045f;

    // Used to determine when the horde should switch direction, updated when destroyed.
    // Uses furthest back enemy in column to minimize number of times needed to search for a new furthest enemy.
    private Transform leftMostEnemy;
    private Transform rightMostEnemy;

    private void Awake()
    {
        BuildEnemyList();
        numberOfEnemies = CalculateNumberOfEnemies();
        Debug.Assert(horizontalMoveAmount > 0, "Horizontal Move Amount is zero.");
        Debug.Assert(verticalMoveAmount > 0, "Vertical Move Amount is zero.");
        Debug.Assert(loseAtYAxis < 0, "Lose At Y Axis is zero or greater.");
        Debug.Assert(spriteDelay > 0, "Sprite Delay is zero.");
        Debug.Assert(rowDelay > 0, "Row Delay is zero.");
        Debug.Assert(tickSpeed > 0, "Tick Speed is zero.");
        Debug.Assert(enemies.Count != 0, "List of enemies is zero.");
        Debug.Assert(waveText, "Wave Text is NULL.");
        Debug.Assert(enemyMissle, "Enemy Middle is NULL.");
        Debug.Assert(minShootSpeed > 0, "Min Shoot Speed is zero.");
        Debug.Assert(maxShootSpeed > 0, "Max Shoot Speed is zero.");
        Debug.Assert(maxShootSpeed > minShootSpeed, "Max Shoot Speed is not higher than Min Shoot Speed.");

        // Wave UI.
        waveText.text = "Wave " + (wave+1).ToString("00");

        // Change stats based on wave.
        BalanceForWave();
    }

    private void Start()
    {
        // Find furthest enemies from each side.s
        leftMostEnemy = GetFurthestMostEnemy(Direction.Left);
        rightMostEnemy = GetFurthestMostEnemy(Direction.Right);

        StartCoroutine(Tick());
        StartCoroutine(Shoot());
    }

    private void BuildEnemyList()
    {
        // Build a list of every enemy class inside the horde in order from bottom to top, left to right.
        foreach (Transform child in transform)
        {
            List<Enemy> row = new List<Enemy>();
            foreach (Transform enemy in child)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent)
                {
                    row.Add(enemyComponent);
                }
            }
            enemies.Add(row);
        }
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickSpeed);
            StartCoroutine(MoveEnemies(movementDirection));
        }
    }

    private IEnumerator MoveEnemies(Direction direction)
    {
        foreach (List<Enemy> row in enemies)
        {
            foreach (Enemy enemy in row)
            {
                if (enemy != null)
                {
                    float offset = direction == Direction.Left ? -horizontalMoveAmount : horizontalMoveAmount;
                    enemy.transform.position = new Vector2(enemy.transform.position.x + offset, enemy.transform.position.y);
                    enemy.FlipSprite();

                    if (spriteDelay > 0)
                    {
                        yield return new WaitForSeconds(spriteDelay);
                    }
                }

            }
            if (rowDelay > 0)
            {
                yield return new WaitForSeconds(rowDelay);
            }
        }

        // Move down and switch directions if reached the side.
        if (direction == Direction.Left)
        {
            if (leftMostEnemy == null)
            {
                leftMostEnemy = GetFurthestMostEnemy(Direction.Left);
            }

            if (leftMostEnemy != null && leftMostEnemy.position.x < -movementClamp)
            {
                MoveEnemiesVertical(Direction.Left);
                movementDirection = Direction.Right;
            }
        }
        else
        {
            if (rightMostEnemy == null)
            {
                rightMostEnemy = GetFurthestMostEnemy(Direction.Right);
            }

            if (rightMostEnemy != null && rightMostEnemy.position.x > movementClamp)
            {
                MoveEnemiesVertical(Direction.Right);
                movementDirection = Direction.Left;
            }
        }
    }

    private void MoveEnemiesVertical(Direction fromDirection)
    {
        // Direction guard.
        if (movementDirection != fromDirection)
        {
            return;
        }

        foreach (List<Enemy> row in enemies)
        {
            foreach (Enemy enemy in row)
            {
                if (enemy != null)
                {
                    enemy.transform.position = new Vector2(enemy.transform.position.x, enemy.transform.position.y - verticalMoveAmount);

                    // Check for losing state.
                    if (enemy.transform.position.y <= loseAtYAxis)
                    {
                        GameManager.Instance.LoseGame();
                    }
                }
            }
        }
    }

    public void EnemyKilled(Enemy enemy)
    {
        numberOfEnemies--;

        // Increase game speed.
        tickSpeed -= tickDecreasePerEnemyDeath;
        if (tickSpeed < minTickSpeed)
        {
            tickSpeed = minTickSpeed;
        }
        spriteDelay -= (tickDecreasePerEnemyDeath/160f);
        if (spriteDelay < 0)
        {
            spriteDelay = 0;
        }
        rowDelay -= (tickDecreasePerEnemyDeath/80f);
        if (rowDelay < 0)
        {
            rowDelay = 0;
        }

        if (numberOfEnemies <= 0)
        {
            // Complete Wave.
            GameManager.Instance.WaveComplete();
        }
    }

    // Taken out of Tick() for balance.
    private IEnumerator Shoot()
    {
        while (numberOfEnemies > 0)
        {
            yield return new WaitForSeconds(Random.Range(minShootSpeed*100, maxShootSpeed*100)/100);

            // The bottom enemy of each column can shoot. 
            List<Enemy> enemiesReadyToFire = EnemiesReadyToFire();

            // Fire missle from enemy.
            if (enemiesReadyToFire.Count > 0)
            {
                Enemy firingEnemy = enemiesReadyToFire[Random.Range(0, enemiesReadyToFire.Count)];
                GameObject newMissle = Instantiate(enemyMissle, firingEnemy.transform.position, Quaternion.identity);
                SpriteRenderer newMissleRenderer = newMissle.GetComponent<SpriteRenderer>();
                if (newMissleRenderer)
                {
                    newMissleRenderer.color = firingEnemy.GetComponent<SpriteRenderer>().color;
                }
            }
        }
    }

    private int CalculateNumberOfEnemies()
    {
        int count = 0;
        foreach (List<Enemy> row in enemies)
        {
            count += row.Count;
        }
        return count;
    }

    // Returns the transform of the enemy on the furthest side of the horde.
    private Transform GetFurthestMostEnemy(Direction direction)
    {
        Transform furthestMostEnemy = null;

        // Diretion Guard.
        if (!(direction == Direction.Left || direction == Direction.Right))
        {
            Debug.LogError("Must be direction left or right.");
            return null;
        }

        // Find enemy at the further left or right and back position.
        foreach (List<Enemy> row in enemies)
        {
            foreach (Enemy enemy in row)
            {
                if (enemy != null && (furthestMostEnemy == null || (direction == Direction.Left ? (enemy.transform.position.x <= furthestMostEnemy.position.x) : (enemy.transform.position.x >= furthestMostEnemy.position.x))))
                {
                    furthestMostEnemy = enemy.transform;
                }
            }
        }

        return furthestMostEnemy;
    }

    // Returns a list of all of the enemies at the bottom of a collumn.
    private List<Enemy> EnemiesReadyToFire()
    {
        // Enemies ordered by their collumn number.
        Dictionary<int, Enemy> readyEnemies = new Dictionary<int, Enemy>();

        for (int y = 0; y < enemies.Count; y++)
        {
            for (int x = 0; x < enemies[y].Count; x++)
            {
                if (!readyEnemies.ContainsKey(x) && enemies[y][x] != null)
                {
                    readyEnemies.Add(x, enemies[y][x]);
                }
            }
        }

        return readyEnemies.Values.ToList<Enemy>();
    }

    // TODO - Improve game balance & remove hard-code values.
    private void BalanceForWave()
    {
        for (int i = 0; i < wave; i++)
        {
            tickSpeed -= 0.02f;
            if (tickSpeed < minTickSpeed)
            {
                tickSpeed = minTickSpeed;
            }

            verticalMoveAmount += 0.01f;
            if (verticalMoveAmount > 0.3f)
            {
                verticalMoveAmount = 0.2f;
            }

            maxShootSpeed -= 0.2f;
            if (maxShootSpeed < minShootSpeed)
            {
                maxShootSpeed = minShootSpeed;
            }
        }
    }

    public static void CompleteWave()
    {
        wave++;
    }

    public static void ResetWaves()
    {
        wave = 0;
    }
}