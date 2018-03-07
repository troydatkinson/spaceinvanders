// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Set scores, returns scores for enemy type.

using UnityEngine;
using System;

[Serializable]
public class ScoreTable
{
    [SerializeField]
    private int weakEnemy = 10;

    [SerializeField]
    private int regularEnemy = 20;

    [SerializeField]
    private int strongEnemy = 30;

    [SerializeField]
    private int mothership = 300;

    // Returns the correct score for the given enemy type.
    public int Score(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
                return 0;
            case EnemyType.Weak:
                return weakEnemy;
            case EnemyType.Regular:
                return regularEnemy;
            case EnemyType.Strong:
                return strongEnemy;
            case EnemyType.Mothership:
                return mothership;
        }
    }
}