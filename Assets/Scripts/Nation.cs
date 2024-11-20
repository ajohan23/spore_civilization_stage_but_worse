using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nation
{
    [SerializeField] string name = "Unnamed Nation";
    [SerializeField] Color teamColor = Color.white;
    public bool isPlayer = true;

    int money = 0;

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public bool HasMoney(int amount)
    {
        return money >= amount;
    }

    public bool RemoveMoney(int amount)
    {
        if (HasMoney(amount))
        {
            money -= amount;
            return true;
        }

        return false;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public int GetMoney()
    {
        return money;
    }
}
