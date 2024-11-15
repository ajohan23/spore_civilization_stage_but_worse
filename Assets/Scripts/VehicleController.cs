using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour, Selectable, Movable
{
    public NavMeshAgent agent;

    Order currentOrder;
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
        print($"{transform.name} was deselected");
    }

    public void OnSelect(Selector selector)
    {
        selector.AddSelection(this);
        print($"{transform.name} was selected");
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

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    void Update()
    {
        if (currentOrder != null)
        {
            currentOrder.Execute(this, transform);
        }
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }
}
