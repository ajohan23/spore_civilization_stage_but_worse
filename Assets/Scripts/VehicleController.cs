using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour, Selectable, Movable, Builder
{
    //Settings
    public NavMeshAgent agent;
    public GameObject selectionIndicator;
    [SerializeField] float buildSpeed = 0.1f;
    [SerializeField] int team = 0;
    [SerializeField] new Renderer renderer;

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
        EnableSelectionIndicator(false);
    }

    public void OnSelect(Selector selector)
    {
        if (selector.GetTeam() != team)
        {
            return;
        }

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

    void Start()
    {
        UpdateColor();
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }

    void EnableSelectionIndicator(bool state)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(state);
        }
    }

    public void Build(Buildable buildable)
    {
        buildable.Build(buildSpeed * Time.deltaTime, team);
    }

    public void SetTeam(int team)
    {
        this.team = team;
        UpdateColor();
    }

    void UpdateColor()
    {
        renderer.material.color = NationsManager.GetNation(team).GetTeamColor();
    }
}
