using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AINationController : MonoBehaviour
{
    [SerializeField] Transform[] goals;
    [SerializeField] int team = 1;
    [SerializeField] int currentGoal = 0;
    [SerializeField] Buildable nextGoal;

    List<VehicleController> threats = new List<VehicleController>();

    Nation nation;

    private void Start()
    {
        nation = NationsManager.GetNation(team);
        nation.onThreatSpottet += AlertToThreat;
    }

    private void Update()
    {
        //Get next goal
        nextGoal = GetNextGoal();
        if (nextGoal != null)
        {
            //Spawn vehicle
            City city = GetNewestCity();
            if (city == null)
                return;

            if (nextGoal.OnLand())
            {
                city.BuyCar();
            }
            else
            {
                city.BuyBoat();
            }

            //Threats and vehicles
            VehicleController[] landThreats = GetThreats(true);
            VehicleController[] seaThreats = GetThreats(false);
            List<VehicleController> availableLandVehicles = new List<VehicleController>(GetVehicles(true));
            List<VehicleController> availableSeaVehicles = new List<VehicleController>(GetVehicles(false));            
            List<VehicleController> unavailableLandVehicles = new List<VehicleController>();
            List<VehicleController> unavailableSeaVehicles = new List<VehicleController>();

            //Deal with land threats
            if (landThreats.Length > 0 && availableLandVehicles.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(landThreats.Length, availableLandVehicles.Count); i++)
                {
                    VehicleController currentThreat = landThreats[i];
                    HealthAttackOrder attackThreat = new HealthAttackOrder(currentThreat, currentThreat.transform, currentThreat.transform.localScale.x * 5);
                    availableLandVehicles[i].ExecuteOrder(attackThreat);
                    unavailableLandVehicles.Add(availableLandVehicles[i]);
                }
                foreach (VehicleController landVehicle in unavailableLandVehicles)
                {
                    availableLandVehicles.Remove(landVehicle);
                }
            }

            //Deal with sea threats
            if (seaThreats.Length > 0 && availableSeaVehicles.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(seaThreats.Length, availableSeaVehicles.Count); i++)
                {
                    VehicleController currentThreat = seaThreats[i];
                    HealthAttackOrder attackThreat = new HealthAttackOrder(currentThreat, currentThreat.transform, currentThreat.transform.localScale.x * 5);
                    availableSeaVehicles[i].ExecuteOrder(attackThreat);
                    unavailableSeaVehicles.Add(availableSeaVehicles[i]);
                }
                foreach (VehicleController seaVehicle in unavailableSeaVehicles)
                {
                    availableSeaVehicles.Remove(seaVehicle);
                }
            }

            //Order
            Transform target = goals[currentGoal].GetChild(0);
            bool goalOnLand = nextGoal.OnLand();
            VehicleController[] selectedVehicles = availableLandVehicles.ToArray();
            if (!goalOnLand)
            {
                selectedVehicles = availableSeaVehicles.ToArray();
            }
            Order order;
            float attackRange = 1.1f;
            if (nextGoal.GetType() == typeof(SpiceGeyser))
            {
                attackRange = 5f;
            }

            if (nextGoal.IsBuild())
            {
                order = new AttackOrder(nextGoal, target, Mathf.Max(target.localScale.x, goals[currentGoal].localScale.z) * attackRange);
            }
            else
            {
                order = new BuildOrder(target.position, nextGoal, Mathf.Max(target.localScale.x, goals[currentGoal].localScale.z) * attackRange);
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
            return null;
        }
        return cities[cities.Length - 1];
    }


    public void AlertToThreat(VehicleController threat)
    {
        if (!threats.Contains(threat))
        {
            threats.Add(threat);
        }
    }

    public void AlertToPriorityThreat(VehicleController threat)
    {
        if (threats.Contains(threat))
            threats.Remove(threat);
        threats.Insert(0, threat);
    }

    public VehicleController[] GetThreats(bool onLand)
    {
        List<VehicleController> threats = new List<VehicleController>();
        List<VehicleController> emptyThreats = new List<VehicleController>();
        foreach(VehicleController threat in this.threats)
        {
            if (threat == null)
            {
                emptyThreats.Add(threat);
                continue;
            }

            if (threat.landVehicle == onLand)
            {
                threats.Add(threat);
            }
        }

        foreach (VehicleController emptyThreat in emptyThreats)
        {
            this.threats.Remove(emptyThreat);
        }
        return threats.ToArray();
    }
}
