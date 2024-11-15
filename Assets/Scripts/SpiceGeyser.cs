using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceGeyser : MonoBehaviour, Buildable
{
    int team = -1;
    bool isBuild = false;
    [SerializeField] GameObject spiceTower;
    [SerializeField] float buildAmount;

    Dictionary<int, float> progression = new Dictionary<int, float>();

    public void Build(float progressAmt, int team)
    {
        if (!progression.ContainsKey(team))
        {
            progression.Add(team, 0);
        }
        progression[team] += progressAmt;

        if (progression[team] >= buildAmount)
        {
            isBuild = true;
        }
    }

    public float GetProgression()
    {
        throw new System.NotImplementedException();
    }

    public int GetTeam()
    {
        return team;
    }

    public bool IsBuild()
    {
        throw new System.NotImplementedException();
    }
}
