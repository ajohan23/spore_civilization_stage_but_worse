using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nation
{
    [SerializeField] string name = "Unnamed Nation";
    [SerializeField] Color teamColor = Color.white;
    public bool isPlayer = true;
    [SerializeField] List<VehicleController> vehicles = new List<VehicleController>();
    [SerializeField] List<City> cities = new List<City>();

    [SerializeField] int money = 1000;
    [SerializeField] float threatAutoRespondRange = 30f;

    int team = -1;

    public delegate void ThreatSpottet(VehicleController threat);
    public ThreatSpottet onThreatSpottet;
    public ThreatSpottet onPriorityThreatSpottet;

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

    public void AddCity(City city)
    {
        if (!cities.Contains(city))
        {
            cities.Add(city);
        }
    }

    public void RemoveCity(City city)
    {
        if (cities.Contains(city))
        {
            cities.Remove(city);
        }
    }

    public City[] GetCities()
    {
        return cities.ToArray();
    }

    public void AddVehicle(VehicleController vehicle)
    {
        if (!vehicles.Contains(vehicle))
        {
            vehicles.Add(vehicle);
        }
    }

    public void RemoveVehicle(VehicleController vehicle)
    {
        if (vehicles.Contains(vehicle))
        {
            vehicles.Remove(vehicle);
        }
    }

    public VehicleController[] GetVehicles()
    {
        return vehicles.ToArray();
    }

    public void SpotThreat(VehicleController threat)
    {
        if (NationsManager.GetNation(threat.GetTeam()) != this)
        {
            onThreatSpottet?.Invoke(threat);
            AutoRespond(threat);
        }
    }

    public void SpotPriorityThreat(VehicleController threat)
    {
        if (threat.GetTeam() != team)
        {
            onPriorityThreatSpottet?.Invoke(threat);
            AutoRespond(threat);
        }
    }

    void AutoRespond(VehicleController threat)
    {
        foreach(VehicleController friendly in vehicles)
        {
            if (Vector3.Distance(friendly.transform.position, threat.transform.position) <= threatAutoRespondRange && friendly.GetCurrentAction() == "Idle")
            {
                friendly.AddImidiateThreat(threat);
            }
        }
    }

    public void CallForHealp(VehicleController threat)
    {
        AutoRespond(threat);
    }

    public string GetName()
    {
        return name;
    }

    public void SetTeam(int team)
    {
        this.team = team;
    }
}
