// Game: Space Invaders Clone
// Author: Troy Atkinson
// Date: 08/01/2018
// Description: Damageable interface, implimented by what can take damaged.

using UnityEngine;

public interface IDamageable
{
    void Damage(GameObject damageDealer);
}