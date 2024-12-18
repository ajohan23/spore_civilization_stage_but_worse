using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface Order
{
    public void Execute(Selectable actor, Transform actorTransform);

    public void Cancel(Selectable actor, Transform actorTransform);
}

/// <summary>
/// Class used to construct and execute a movement on a Moviable
/// </summary>
public class MoveOrder : Order
{
    Vector3 targetDestination;
    float cancelDistance = 0.1f;
    IdleOrder idleOrder = new IdleOrder();

    public MoveOrder(Vector3 targetDestination, float cancelDistance)
    {
        this.targetDestination = targetDestination;
        this.cancelDistance = cancelDistance;
    }

    public void Cancel(Selectable actor, Transform actorTransform)
    {
        idleOrder.Execute(actor, actorTransform);
    }

    public void Execute(Selectable actor, Transform actorTransform)
    {
        string actionString = $"Moving to {targetDestination.x}, {targetDestination.y}, {targetDestination.z}";

        if (actor.GetCurrentAction() != actionString)
        {
            Movable actorMovable = actorTransform.GetComponent<Movable>();
            if (actorMovable != null)
            {
                actorMovable.SetDestination(targetDestination);
                actor.SetCurrentAction(actionString);
            }
        }
        else if (Vector3.Distance(actorTransform.position, targetDestination) <= cancelDistance)
        {
            actor.CancelCurrentOrder();
        }
    }
}

public class IdleOrder : Order
{
    public void Cancel(Selectable actor, Transform actorTransform)
    {
        MakeActorIdle(actor, actorTransform);
    }

    public void Execute(Selectable actor, Transform actorTransform)
    {
        MakeActorIdle(actor, actorTransform);
    }

    void MakeActorIdle(Selectable actor, Transform actorTransform)
    {
        Movable actorMovable = actorTransform.GetComponent<Movable>(); 
        if (actorMovable != null)
        {
            actorMovable.StopMoving();
            actor.SetCurrentAction("Idle");
        }
    }
}

public class BuildOrder : Order
{
    Buildable building;
    Vector3 buildingLocation;
    float buildDistance;
    MoveOrder moveOrder;
    IdleOrder idleOrder = new IdleOrder();

    public BuildOrder(Vector3 buildingLocation, Buildable building, float buildDistance)
    {
        this.buildingLocation = buildingLocation;
        this.building = building;
        this.buildDistance = buildDistance;
        moveOrder = new MoveOrder(buildingLocation, buildDistance);
    }

    public void Cancel(Selectable actor, Transform actorTransform)
    {
        idleOrder.Execute(actor, actorTransform);
    }

    public void Execute(Selectable actor, Transform actorTransform)
    {
        Builder actorBuilder = actorTransform.GetComponent<Builder>();
        if (actorBuilder == null)
        {
            actor.CancelCurrentOrder();
            return;
        }

        if (Vector3.Distance(actorTransform.position, buildingLocation) <= buildDistance)
        {
            moveOrder.Cancel(actor, actorTransform);
            actorBuilder.Build(building);
            actor.SetCurrentAction("Building");

            if (building.IsBuild())
            {
                actor.CancelCurrentOrder();
            }
        }
        else
        {
            moveOrder.Execute(actor, actorTransform);
        }
    }
}

public class AttackOrder : Order
{
    Buildable target;
    Transform targetTransform;
    float attackDistance;
    MoveOrder moveOrder;
    IdleOrder idleOrder = new IdleOrder();

    public AttackOrder(Buildable target, Transform targetTransform, float attackDistance)
    {
        this.target = target;
        this.targetTransform = targetTransform;
        this.attackDistance = attackDistance;
        moveOrder = new MoveOrder(this.targetTransform.position, attackDistance);
    }

    public void Cancel(Selectable actor, Transform actorTransform)
    {
        actor.ExecuteOrder(idleOrder, false);
    }

    public void Execute(Selectable actor, Transform actorTransform)
    {
        if (Vector3.Distance(actorTransform.position, targetTransform.position) > attackDistance)
        {
            moveOrder.Execute(actor, actorTransform);
        }
        else
        {
            moveOrder.Cancel(actor, actorTransform);
            Attacker attacker = actorTransform.GetComponent<Attacker>();
            if (attacker != null)
            {
                attacker.Attack(targetTransform);
            }
            else
            {
                Cancel(actor, targetTransform);
            }
        }
    }
}

public class HealthAttackOrder : Order
{
    Health target;
    Transform targetTransform;
    float attackDistance;
    MoveOrder moveOrder;
    IdleOrder idleOrder = new IdleOrder();

    public HealthAttackOrder(Health target, Transform targetTransform, float attackDistance)
    {
        this.target = target;
        this.targetTransform = targetTransform;
        this.attackDistance = attackDistance;
    }
    public void Cancel(Selectable actor, Transform actorTransform)
    {
        actor.ExecuteOrder(idleOrder, false);
    }

    public void Execute(Selectable actor, Transform actorTransform)
    {
        if (targetTransform == null || target == null)
        {
            Cancel(actor, actorTransform);
            return;
        }

        if (Vector3.Distance(actorTransform.position, targetTransform.position) > attackDistance)
        {
            moveOrder = new MoveOrder(targetTransform.position, attackDistance);
            moveOrder.Execute(actor, actorTransform);
        }
        else
        {
            Movable actorMovable = actorTransform.GetComponent<Movable>();
            if (actorMovable != null)
            {
                actorMovable.StopMoving();
            }
            Attacker attacker = actorTransform.GetComponent<Attacker>();
            if (attacker != null)
            {
                attacker.Attack(targetTransform);
            }
            else
            {
                Cancel(actor, targetTransform);
            }
        }
    }
}

public interface Movable
{
    public void SetDestination(Vector3 destination);

    public void StopMoving();
}

public interface Builder
{
    public void Build(Buildable buildable);
}

public interface Buildable
{
    public void Build(float progressAmt, int team, VehicleController builder);

    public bool IsBuild();

    public float GetProgression();

    public int GetTeam();

    public void Destroy();

    public bool OnLand();
}