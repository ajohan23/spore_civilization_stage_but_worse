using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBillboard : MonoBehaviour
{
    [SerializeField] Transform billboard;

    private void Update()
    {
        if (billboard != null)
        {
            billboard.forward = Camera.main.transform.forward;
            billboard.position = transform.position + (Camera.main.transform.position - transform.position).normalized * transform.localScale.z/2;
        }
    }
}
