using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiceGeyser : MonoBehaviour, Buildable
{
    int team = -1;
    [SerializeField] GameObject spiceTower;
    [SerializeField] float buildAmount = 2;

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
            this.team = team;
            EnableBuilding(true);
        }
    }

    public void Destroy()
    {
        EnableBuilding(false);
    }

    public float GetProgression()
    {
        return progression.Values.Max();
    }

    public int GetTeam()
    {
        return team;
    }

    public bool IsBuild()
    {
        return spiceTower.activeSelf;
    }

    void EnableBuilding(bool state)
    {
        spiceTower.SetActive(state);
        progression.Clear();
    }
}
