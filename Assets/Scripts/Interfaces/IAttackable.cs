using System;
using UnityEngine;

public interface IAttackable
{
    /// <summary>
    /// Called from the attacking player to the attacked player
    /// </summary>
    public void Attacked(Vector3 knockbackDir);
}
