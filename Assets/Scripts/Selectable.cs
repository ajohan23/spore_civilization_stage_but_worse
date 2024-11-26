using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Selectable
{
    public void OnDeselect();

    public void OnSelect(Selector selector);

    public void ExecuteOrder(Order order, bool overrideThreats);

    public void CancelCurrentOrder();

    public string GetCurrentAction();

    public void SetCurrentAction(string action);

    public MovementType GetMovementType();
}

public enum MovementType
{
    None,
    Land,
    Sea
}
