using System;
using UnityEngine;

public interface IAttackable
{
    /// <summary>
    /// Called from the attacking player to the attacked player
    /// </summary>
    public void Attacked(Vector3 knockbackDir);

    /// <summary>
    /// If the player picks up a shield this sets a bool within combat/health to true
    /// </summary>
    /// <param name="shield"></param>
    public void SetShield(bool shield);
}
