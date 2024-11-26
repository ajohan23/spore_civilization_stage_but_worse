using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour, Selectable, Buildable
{
    [SerializeField] int team = 0;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] Transform carSpawnpoint;
    [SerializeField] Transform carRallypoint;
    MoveOrder carRallypointOrder;
    [SerializeField] GameObject carPrefab;
    [SerializeField] int carPrice = 1000;
    [SerializeField] Transform boatSpawnpoint;
    [SerializeField] Transform boatRallypoint;
    MoveOrder boatRallypointOrder;
    [SerializeField] GameObject boatPrefab;
    [SerializeField] int boatPrice = 1500;
    [SerializeField] float maxHealth = 1000f;
    [SerializeField] bool onLand = true;
    [SerializeField] float generationDelay = 5f;
    float timeToMoney = 0f;
    [SerializeField] int moneyGenerated = 120;

    Dictionary<int, float> progression = new Dictionary<int, float>();

    Order currentOrder = null;
    string currentAction = "Idle";
    Nation nation;

    void Start()
    {
        nation = NationsManager.GetNation(team);
        if (nation == null )
        {
            Debug.Log($"Error nation not found {team}");
            return;
        }
        UpdateColor();
        nation.AddCity(this);

        carRallypointOrder = new MoveOrder(carRallypoint.position, 5f);
        boatRallypointOrder = new MoveOrder(boatRallypoint.position, 5f);
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

    public void CancelCurrentOrder()
    {
        currentOrder.Cancel(this, transform);
        currentOrder = null;
        currentAction = "Idle";
    }

    public void ExecuteOrder(Order order)
    {
        currentOrder = order;
    }

    public void OnDeselect()
    {
        EnableSelectionIndicator(false);
    }

    public void OnSelect(Selector selector)
    {
        if (selector.GetTeam() != team)
        {
            return;
        }
        selector.DeselectAll();
        selector.AddSelection(this);
        EnableSelectionIndicator(true);
    }

    /// <summary>
    /// Returns the current action.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentAction()
    {
        return currentAction;
    }

    /// <summary>
    /// Sets the current action to the given string.
    /// </summary>
    /// <param name="action"></param>
    public void SetCurrentAction(string action)
    {
        currentAction = action;
    }

    void EnableSelectionIndicator(bool state)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(state);
        }
    }

    public void BuyCar()
    {
        if (nation == null)
            return;

        if (!nation.RemoveMoney(carPrice))
            return;

        GameObject newCar = Instantiate(carPrefab, carSpawnpoint.position, Quaternion.identity);
        VehicleController car = newCar.GetComponent<VehicleController>();
        if (car != null)
        {
            car.SetTeam(team);
            car.ExecuteOrder(carRallypointOrder);
        }
    }

    public void BuyBoat()
    {
        if (nation == null)
            return;

        if (!nation.RemoveMoney(boatPrice))
            return;

        GameObject newBoat = Instantiate(boatPrefab, boatSpawnpoint.position, Quaternion.identity);
        VehicleController boat = newBoat.GetComponent<VehicleController>();
        if (boat != null)
        {
            boat.SetTeam(team);
            boat.ExecuteOrder(boatRallypointOrder);
        }
    }

    public int GetTeam()
    {
        return team;
    }

    public void Build(float progressAmt, int team, VehicleController attacker)
    {
        if (!progression.ContainsKey(team))
        {
            progression.Add(team, 0);
        }
        progression[team] += progressAmt;

        
        if (attacker != null)
        {
            print("City was attacked!");
            nation.SpotThreat(attacker);
        }

        if (progression[team] >= maxHealth)
        {
            nation.RemoveCity(this);
            this.team = team;
            nation = NationsManager.GetNation(team);
            nation.AddCity(this);
            UpdateColor();
        }
    }

    public bool IsBuild()
    {
        return true;
    }

    public float GetProgression()
    {
        return progression.Values.Max();
    }

    public void Destroy()
    {
        Debug.Log("City cannot be destroyed");
    }

    void UpdateColor()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.color = nation.GetTeamColor();
    }

    public MovementType GetMovementType()
    {
        return MovementType.None;
    }

    public bool OnLand()
    {
        return onLand;
    }
}
