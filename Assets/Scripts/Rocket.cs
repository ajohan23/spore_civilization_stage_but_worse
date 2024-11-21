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

    Transform target;
    int team = 0;
    Rigidbody rb;
    Vector3 currentTarget;

    public void SetTargetAndTeam(Transform target, int team)
    {
        this.target = target;
        this.team = team;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        currentTarget = transform.position + new Vector3(0, flightPathTop, 0);
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(currentTarget, transform.position) <= targetChangeDistance)
        {
            currentTarget = target.position;
        }
        Vector3 targetForwardVector = currentTarget - transform.position;
        transform.forward = Vector3.Slerp(transform.forward, targetForwardVector, turningSpeed);
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        Buildable buildable = collision.transform.GetComponent<Buildable>();
        if (buildable != null)
        {
            buildable.Build(damage, team);
        }
        Health health = collision.transform.GetComponent<Health>();
        if (health != null)
        {
            health.DealDamage(damage);
        }

        Destroy(gameObject);
    }
}
