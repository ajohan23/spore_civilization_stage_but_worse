using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    public float speed = 2f;
    public float flightPathTop = 15f;
    public float targetChangeDistance = 0.5f;
    public GameObject explosion;
    public float damage = 10f;
    public float turningSpeed = 0.5f;
    public VehicleController owner;

    bool homing = false;
    Transform target;
    int team = 0;
    Rigidbody rb;
    Vector3 currentTarget;

    public void SetTargetAndTeam(Transform target, int team, VehicleController owner)
    {
        this.target = target;
        this.team = team;
        this.owner = owner;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        currentTarget = transform.position + new Vector3(0, flightPathTop, 0);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Explode();
            return;
        }

        if (Vector3.Distance(currentTarget, transform.position) <= targetChangeDistance || homing == true)
        {
            homing = true;
            currentTarget = target.position;
        }
        Vector3 targetForwardVector = currentTarget - transform.position;
        transform.forward = Vector3.Slerp(transform.forward, targetForwardVector, turningSpeed);
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {

        Buildable buildable = collision.transform.GetComponent<Buildable>();
        if (buildable != null)
        {
            buildable.Build(damage, team, owner);
        }

        Health health = collision.transform.GetComponent<Health>();
        if (health != null)
        {
            health.DealDamage(damage, owner);
        }

        Explode();
    }

    void Explode()
    {

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
