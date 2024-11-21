using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour, Selectable, Movable, Builder, Attacker
{
    //Settings
    public NavMeshAgent agent;
    public GameObject selectionIndicator;
    [SerializeField] float buildSpeed = 0.1f;
    [SerializeField] int team = 0;
    [SerializeField] new Renderer renderer;
    [SerializeField] GameObject rocket;
    [SerializeField] Transform rocketBay;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] MovementType movementType = MovementType.Land;

    Order currentOrder;
    string currentAction = "Idle";
    float reloadCooldown = 0f;

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

        if (reloadCooldown > 0f)
        {
            reloadCooldown -= Time.deltaTime;
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

    public void Attack(Transform target)
    {
        Buildable targetBuilding = target.GetComponent<Buildable>();
        if (targetBuilding != null)
        {
            if (targetBuilding.GetTeam() == this.team)
            {
                CancelCurrentOrder();
            }
        }

        if (reloadCooldown <= 0f)
        {
            GameObject newRocket = Instantiate(rocket, rocketBay.position, rocketBay.rotation);
            Rocket _rocket = newRocket.GetComponent<Rocket>();
            _rocket.SetTargetAndTeam(target, team);
            reloadCooldown = reloadTime;
        }
    }

    public MovementType GetMovementType()
    {
        return movementType;
    }
}
