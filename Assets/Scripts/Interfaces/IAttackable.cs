using System;
using UnityEngine;

public interface IAttackable
{
    /// <summary>
    /// Called through 
    /// </summary>
    public void Attacked(Vector3 knockbackDir);
}
