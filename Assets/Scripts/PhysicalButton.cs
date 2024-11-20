using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicalButton : MonoBehaviour
{
    public Material enabledMaterial;
    public Material disabledMaterial;
    public new Renderer renderer;

    protected bool interactable = true;

    public void SetInteractable(bool state)
    {
        interactable = state;

        if (interactable)
        {
            renderer.material = enabledMaterial;
        }
        else
        {
            renderer.material = disabledMaterial;
        }
    }

    public virtual void Press()
    {

    }
}
