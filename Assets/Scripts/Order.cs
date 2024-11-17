using System.Collections;
using System.Collections.Generic;
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
    public void Build(float progressAmt, int team);

    public bool IsBuild();

    public float GetProgression();

    public int GetTeam();

    public void Destroy();
}

