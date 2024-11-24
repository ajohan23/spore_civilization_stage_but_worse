using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AINationController : MonoBehaviour
{
    [SerializeField] Transform[] goals;
    [SerializeField] int team = 1;
    [SerializeField] float ActionTime = 60f;
    [SerializeField] int currentGoal = 0;
    [SerializeField] Buildable nextGoal;

    float time2Action = 0f;
    Nation nation;

    private void Start()
    {
        nation = NationsManager.GetNation(team);
    }

    private void Update()
    {
        //Get next goal
        nextGoal = GetNextGoal();
        if (nextGoal != null)
        {
            //Spawn vehicle
            City city = GetNewestCity();
            if (nextGoal.OnLand())
            {
                city.BuyCar();
            }
            else
            {
                city.BuyBoat();
            }

            //Order
            Transform target = goals[currentGoal].GetChild(0);
            bool goalOnLand = nextGoal.OnLand();
            VehicleController[] selectedVehicles = GetVehicles(goalOnLand);
            Order order;
            if (nextGoal.IsBuild())
            {
                order = new AttackOrder(nextGoal, target, Mathf.Max(target.localScale.x, goals[currentGoal].localScale.z) * 5);
            }
            else
            {
                order = new BuildOrder(target.position, nextGoal, Mathf.Max(target.localScale.x, goals[currentGoal].localScale.z) * 5);
            }

            foreach (VehicleController vehicle in selectedVehicles)
            {
                vehicle.ExecuteOrder(order);
            }
        }
    }

    Buildable GetNextGoal()
    {
        for(int i = 0; i < goals.Length; i++)
        {
            Buildable goal = goals[i].GetComponentInChildren<Buildable>();
            if (goal != null)
            {
                if (goal.GetTeam() != team)
                {
                    currentGoal = i;
                    return goal;
                }
            }
        }
        return null;
    }

    VehicleController[] GetVehicles(bool isLand)
    {
        List<VehicleController> vehicles = new List<VehicleController>();
        foreach (VehicleController vehicle in nation.GetVehicles())
        {
            if (vehicle.landVehicle == isLand)
            {
                vehicles.Add(vehicle);
            }
        }
        return vehicles.ToArray();
    }

    City GetNewestCity()
    {
        City[] cities = nation.GetCities();
        if (cities.Length == 0)
        {
            enabled = false;
        }
        return cities[cities.Length - 1];
    }
}
