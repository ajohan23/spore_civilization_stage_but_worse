using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Health
{
    public void DealDamage(float damage);

    public float GetRemainingHealth();

    public float GetMaxHealth();

    public int GetTeam();
}

public interface Attacker
{
    public void Attack(Transform target);
}