using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiceGeyser : MonoBehaviour, Buildable
{
    int team = -1;
    [SerializeField] GameObject spiceTower;
    [SerializeField] float buildAmount = 2;
    [SerializeField] new Renderer renderer;
    [SerializeField] float generationDelay = 10f;
    float timeToMoney = 0f;
    [SerializeField] int moneyGenerated = 200;
    [SerializeField] bool onLand = true;

    Dictionary<int, float> progression = new Dictionary<int, float>();

    public void Build(float progressAmt, int team, VehicleController attacker)
    {
        if (!progression.ContainsKey(team))
        {
            progression.Add(team, 0);
        }
        progression[team] += progressAmt;

        if (progression[team] >= buildAmount)
        {
            this.team = team;
            BuildTower();
        }

        if (IsBuild())
        {
            if (attacker != null)
            {
                NationsManager.GetNation(team).SpotThreat(attacker);
            }
        }
    }

    public void Destroy()
    {
        EnableBuilding(false);
        team = -1;
        progression.Clear();
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

    void UpdateColor()
    {
        if (renderer != null)
        {
            renderer.material.color = NationsManager.GetNation(team).GetTeamColor();
        }
    }

    void Update()
    {
        if (IsBuild())
        {
            timeToMoney -= Time.deltaTime;
            if (timeToMoney <= 0)
            {
                NationsManager.GetNation(team).AddMoney(moneyGenerated);
                timeToMoney = generationDelay;
            }
        }
    }

    void BuildTower()
    {
        UpdateColor();
        EnableBuilding(true);
    }

    public bool OnLand()
    {
        return onLand;
    }
}
