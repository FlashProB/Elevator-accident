﻿using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemyEasyAI : MonoBehaviour
{
    [Header("Patrolling")]
    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Physics")]
    public float speed;

    [Header("Stupid shit")]
    Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;

    public void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
    }

    private void Update()
    {
        Vector2 point = currentPoint.position - transform.position;
        if (currentPoint == pointB.transform && speed > 0f)
        {
            anim.SetBool("Moving", true);
            rb.velocity = new Vector2(speed, 0);
        }
        else if (currentPoint == pointA.transform && speed > 0f)
        {
            anim.SetBool("Moving", true);
            rb.velocity = new Vector2(-speed, 0);
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            anim.SetBool("Moving", false);
            speed = 0f;
            currentPoint = pointA.transform;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            anim.SetBool("Moving", false);
            speed = 0f;
            currentPoint = pointB.transform;
        }
        Flip();
    }

    private void Flip()
    {
        if (rb.velocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}