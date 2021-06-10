using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int initial = 0;
    private int maximum = 0;
    private int current = 0;

    public int Current { get { return current; } }
    public int Maximum { get { return maximum; } }

    private void Awake()
    {
        maximum = initial;
        current = initial;
    }

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
