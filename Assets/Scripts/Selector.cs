using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] int team = 0;
    [SerializeField] LayerMask clickMask;
    List<Selectable> selected = new List<Selectable>();

    private void Update()
    {
        LeftClick();
        RightClick();
    }

    void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickMask))
            {
                //If we hit a selectable add it to the selected elements
                Selectable selectable = hit.transform.GetComponent<Selectable>();
                if (selectable != null)
                {
                    if (!Input.GetKey(KeyCode.LeftShift)) 
                    {
                        DeselectAll();
                    }
                    selectable.OnSelect(this);
                    return;
                }

                PhysicalButton button = hit.transform.GetComponent<PhysicalButton>();
                if (button != null)
                {
                    button.Press();
                    return;
                }

                DeselectAll();
            }
        }
    }

    void RightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //If there is anything we can order
            if (selected.Count > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray,out hit, Mathf.Infinity, clickMask))
                {

                    switch (hit.transform.tag)
                    {
                        case "Ground":
                            foreach (Selectable _selected in selected)
                            {
                                _selected.ExecuteOrder(CreateMoveOrder(hit.point));
                            }
                            break;
                        case "Spice Geyser":
                            print("Spice");
                            Buildable geyserBuildable = hit.transform.GetComponent<Buildable>();
                            if (geyserBuildable != null)
                            {
                                if (geyserBuildable.IsBuild())
                                {
                                   if (geyserBuildable.GetTeam() != team)
                                    {
                                        //TODO: Issuee attack order
                                    }
                                }
                                else
                                {
                                    print("Issuing build order");
                                    BuildOrder buildOrder = new BuildOrder(hit.transform.position, geyserBuildable, Mathf.Max(hit.transform.localScale.x, hit.transform.localScale.z) * 5);
                                    foreach (Selectable _selected in selected)
                                    {
                                        _selected.ExecuteOrder(buildOrder);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    MoveOrder CreateMoveOrder(Vector3 targetDestination)
    {
        return new MoveOrder(targetDestination, Mathf.Abs(selected.Count * 5));
    }

    public void Deselect(Selectable selectable)
    {
        if (selected.Contains(selectable))
        {
            selectable.OnDeselect();
            selected.Remove(selectable);
        }
    }

    public void DeselectAll()
    {
        foreach (Selectable selectable in selected)
        {
            selectable.OnDeselect();
        }
        selected.Clear();
    }

    public void AddSelection(Selectable selectable)
    {
        if (!selected.Contains(selectable))
            selected.Add(selectable);
    }

    public int GetTeam()
    {
        return team;
    }

    public Selectable[] GetSelected()
    {
        return selected.ToArray();
    }
}
