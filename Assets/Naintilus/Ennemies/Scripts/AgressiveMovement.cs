using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgressiveMovement : MonoBehaviour
{
    public Transform player;
    public float regularSpeed = 1;
    public float dashSpeed = 2;
    public float reactionDistance = 2;
    public float reactionTime = .5f;

    private Rigidbody rb;
    private bool hit;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartCoroutine(MovementCoroutine());
    }

    private void Update() {
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private IEnumerator MovementCoroutine()
    {
        yield return MoveTowardPlayer();
        yield return ReactToPlayer();
        yield return DashTowardPlayer();
    }

    private IEnumerator MoveTowardPlayer()
    {
        rb.velocity = transform.forward * regularSpeed;
        while((player.position - transform.position).magnitude > reactionDistance)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ReactToPlayer()
    {
        //play sfx
        //display exclamation mark
        transform.forward = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(reactionTime);
    }

    private IEnumerator DashTowardPlayer()
    {
        //play vfx + sfx
        rb.velocity = transform.forward * dashSpeed;
        while(Vector3.Dot((player.position - transform.position), transform.forward) > 0 || hit == false)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter(Collision other) {
        //EXPLOSION
    }
}
