using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour, Selectable, Movable, Builder, Attacker, Health
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
    public bool landVehicle = true;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float imidiateThreatRange = 8f;
    [SerializeField] float imidiateThreatFollowRange = 16f;
    [SerializeField] float overrideCooldownTime = 30f;

    float overrideCooldown = 0f;
    float currentHealth = 100;
    Order currentOrder;
    string currentAction = "Idle";
    float reloadCooldown = 0f;
    List<VehicleController> imidiateThreats = new List<VehicleController>();
    Nation nation;

    public void CancelCurrentOrder()
    {
        currentOrder.Cancel(this, transform);
        currentOrder = null;
        currentAction = "Idle";
        overrideCooldown = 0f;
    }

    public void ExecuteOrder(Order order, bool overrideThreats)
    {
        if (overrideThreats)
        {
            imidiateThreats.Clear();
            overrideCooldown = overrideCooldownTime;
        }

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
        if (imidiateThreats.Count > 0 && overrideCooldown > 0)
        {
            RespondToImidiateThreats();
        }
        else if (currentOrder != null)
        {
            currentOrder.Execute(this, transform);
        }

        if (reloadCooldown > 0f)
        {
            reloadCooldown -= Time.deltaTime;
        }

        if (overrideCooldown > 0f)
        {
            overrideCooldown -= Time.deltaTime;
        }


        LocateImidiateThreats();
        CheckDistancesToThreats();
    }

    void Start()
    {
        currentHealth = maxHealth;
        NationsManager.GetNation(team).AddVehicle(this);
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
        buildable.Build(buildSpeed * Time.deltaTime, team, this);
    }

    public void SetTeam(int team)
    {
        this.team = team;
        nation = NationsManager.GetNation(team);
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
            _rocket.SetTargetAndTeam(target, team, this);
            reloadCooldown = reloadTime;
        }
    }

    public MovementType GetMovementType()
    {
        return movementType;
    }

    private void OnDestroy()
    {
        NationsManager.GetNation(team).RemoveVehicle(this);
    }

    public void DealDamage(float damage, VehicleController attacker)
    {
        currentHealth -= damage;

        if (currentHealth < 0f)
        {
            Die();
        }

        if (attacker == null)
            return;

        if (attacker != null)
        {
            imidiateThreats.Add(attacker);
            nation.CallForHealp(attacker);
        }
    }

    public float GetRemainingHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetTeam()
    {
        return team;
    }

    void Die()
    {
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void LocateImidiateThreats()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, imidiateThreatRange);

        foreach (Collider collider in colliders)
        {
            VehicleController vehicle = collider.GetComponent<VehicleController>();
            if (vehicle != null)
            {
                if (vehicle.GetTeam() != team)
                {
                    if (team == 0) //Only for debugging
                    {
                        print("Threat detected");
                    }
                    AddImidiateThreat(vehicle);
                }
            }
        }
    }

    void CheckDistancesToThreats()
    {
        for(int i = 0; i < imidiateThreats.Count; i++)
        {
            if (imidiateThreats[i] == null)
                continue;

            if (Vector3.Distance(imidiateThreats[i].transform.position, transform.position) > imidiateThreatFollowRange)
            {
                imidiateThreats[i] = null;
            }
        }
    }

    void RespondToImidiateThreats()
    {
        if (imidiateThreats[0] == null)
        {
            imidiateThreats.RemoveAt(0);
            return;
        }

        if (imidiateThreats.Count > 0)
        {
            HealthAttackOrder response = new HealthAttackOrder(imidiateThreats[0], imidiateThreats[0].transform, imidiateThreats[0].transform.localScale.x * 8);
            response.Execute(this, transform);
        }
    }

    public void AddImidiateThreat(VehicleController threat)
    {
        if (imidiateThreats.Contains(threat) == false)
        {
            imidiateThreats.Add(threat);
        }
    }

    public void RemoveImidiateThreat(VehicleController threat)
    {
        if (imidiateThreats.Contains(threat))
        {
            imidiateThreats.Remove(threat);
        }
    }
}
