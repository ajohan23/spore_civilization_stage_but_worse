using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour, Selectable
{
    [SerializeField] int team = 0;
    [SerializeField] GameObject selectionIndicator;
    [SerializeField] Transform carSpawnpoint;
    [SerializeField] Transform carRallypoint;
    [SerializeField] GameObject carPrefab;


    Order currentOrder = null;
    string currentAction = "Idle";

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
        GameObject newCar = Instantiate(carPrefab, carSpawnpoint.position, Quaternion.identity);
        VehicleController car = newCar.GetComponent<VehicleController>();
        if (car != null)
        {
            car.SetTeam(team);
            car.SetDestination(carRallypoint.position);
        }
    }
}
