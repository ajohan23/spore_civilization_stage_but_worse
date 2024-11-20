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
    [SerializeField] float maxHealth = 1000f;

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

        carRallypointOrder = new MoveOrder(carRallypoint.position, 5f);
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

        if (CameraController.instance != null)
        {
            CameraController.instance.LookAt(transform.position);
        }
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

    public int GetTeam()
    {
        return team;
    }

    public void Build(float progressAmt, int team)
    {
        if (!progression.ContainsKey(team))
        {
            progression.Add(team, 0);
        }
        progression[team] += progressAmt;

        if (progression[team] >= maxHealth)
        {
            this.team = team;
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
}
