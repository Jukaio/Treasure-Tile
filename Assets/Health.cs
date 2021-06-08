using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : ScriptableObject
{
    [SerializeField] private int initial = 0;
    [SerializeField] private int maximum = 0;
    [SerializeField] private int current = 0;

    public int Current { get { return current; } }

    public bool IsDead()
    {
        return current <= 0;
    }
    public bool IsAlive()
    {
        return !IsDead();
    }

    public void Reduce(int amount)
    {
        current = Mathf.Clamp(current - amount, 0, maximum);
    }

    public bool ReduceAndCheckDeath(int amount)
    {
        Reduce(amount);
        return IsDead();
    }

    public void Increase(int amount)
    {
        current = Mathf.Clamp(current + amount, 0, maximum);
    }


}
