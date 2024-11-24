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
}
