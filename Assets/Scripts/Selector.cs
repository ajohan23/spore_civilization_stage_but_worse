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
                            MoveOrder moveOrder = CreateMoveOrder(hit.point);
                            foreach (Selectable _selected in selected)
                            {
                                if(_selected.GetMovementType() == MovementType.Land)
                                {
                                    _selected.ExecuteOrder(moveOrder, true);
                                }
                            }
                            break;
                        case "Water":
                            moveOrder = CreateMoveOrder(hit.point);
                            foreach (Selectable _selected in selected)
                            {
                                if(_selected.GetMovementType() == MovementType.Sea)
                                {
                                    _selected.ExecuteOrder(moveOrder, true);
                                }
                            }
                            break;
                        case "Spice Geyser":
                            Buildable geyserBuildable = hit.transform.GetComponent<Buildable>();
                            if (geyserBuildable != null)
                            {
                                if (geyserBuildable.IsBuild())
                                {
                                   if (geyserBuildable.GetTeam() != team)
                                    {
                                        AttackOrder attackOrder = new AttackOrder(geyserBuildable, hit.transform, Mathf.Max(hit.transform.localScale.x, hit.transform.localScale.z) * 5);
                                        foreach (Selectable _selected in selected)
                                        {
                                            _selected.ExecuteOrder(attackOrder, true);
                                        }
                                    }
                                }
                                else
                                {
                                    BuildOrder buildOrder = new BuildOrder(hit.transform.position, geyserBuildable, Mathf.Max(hit.transform.localScale.x, hit.transform.localScale.z) * 5);
                                    foreach (Selectable _selected in selected)
                                    {
                                        _selected.ExecuteOrder(buildOrder, true);
                                    }
                                }
                            }
                            break;
                        case "City":
                            Buildable city = hit.transform.GetComponent<Buildable>();
                            if (city == null)
                                return;
                            if (city.GetTeam() != team)
                            {
                                AttackOrder attackOrder = new AttackOrder(city, hit.transform, Mathf.Max(hit.transform.localScale.x, hit.transform.localScale.z) * 1.1f);
                                foreach (Selectable _selected in selected)
                                {
                                    _selected.ExecuteOrder(attackOrder, true);
                                }
                            }
                            break;
                        case "Vehicle":
                            Health health = hit.transform.GetComponent<Health>();
                            if (health == null) 
                                return;
                            if (health.GetTeam() != team)
                            {
                                HealthAttackOrder order = new HealthAttackOrder(health, hit.transform, Mathf.Max(hit.transform.localScale.x, hit.transform.localScale.z) * 8f);
                                foreach (Selectable _selected in selected)
                                {
                                    _selected.ExecuteOrder(order, true);
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
