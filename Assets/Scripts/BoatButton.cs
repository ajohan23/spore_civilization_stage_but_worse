using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatButton : PhysicalButton
{
    public City city;

    public override void Press()
    {
        if (interactable)
        {
            city.BuyBoat();
        }
    }
}
