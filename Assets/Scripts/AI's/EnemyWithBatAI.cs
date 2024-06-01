using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemyWithBatAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    private Health playerHealth;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom")]
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] Vector3 startOffset;
    [SerializeField] private float colliderDistance;
    [SerializeField] private float attackCoodown;
    private float cooldownTimer = Mathf.Infinity;

    private Path path;
    private int currentWaypoint = 0;
    [SerializeField] public RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    private SpriteRenderer sprite;
    public bool flip;
    private Animator anim;
    private bool isOnCoolDown;
    private BoxCollider2D coll;
    private float speedofwalking;
    public bool NoPlayer;
    public bool Walking;
    public GameObject player;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float range;
    [SerializeField] private LayerMask playerLayer;
    public Transform groundDetection;

    private enum MovementState { running, jumping, falling }

    public void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        speed = 0f;
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsFalling", false);
        NoPlayer = true;
        Walking = false;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCoodown)
            {
                cooldownTimer = 0;
                if (NoPlayer == false && Walking == true)
                {
                    anim.SetTrigger("Attack");
                    Walking = false;
                    NoPlayer = true;
                }
                if (Walking == true)
                {
                    anim.SetBool("FoundPlayer", false);
                }
                else if (Walking == false)
                {
                    anim.SetBool("FoundPlayer", true);
                }

            }
        }
       

        if (Walking)
        {
            Vector3 scale = transform.localScale;
            if (player.transform.position.x > transform.position.x)
            {
                scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
            }
            transform.localScale = scale;
        }

        UpdateAnimationState();
    }
    private void FixedUpdate()
    {
        if (NoPlayer == false)
        {
            if (TargetInDistance())
            {
                PathFollow();
            }
        }
    }
    private void Hurt()
    {
        Vector3 scale = transform.localScale;
        scale.x = -1;
        anim.SetBool("Sitting", false);
        anim.SetBool("Walking", true);
        anim.SetBool("FoundPlayer", false);
        speed = 6;
        Walking = true;
        NoPlayer = false;
    }
    private void FoundPlayer()
    {
        sprite.flipX = false;
        anim.SetBool("Sitting", false);
        anim.SetBool("Walking", true);
        speed = 6;
        Walking = true;
        NoPlayer = false;
    }
    private void HitPlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(1);
        }
    }
    private void Sit()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * 1;
        transform.localScale = scale;
        anim.SetBool("Sitting", true);
        anim.SetBool("FoundPlayer", false);
        anim.SetBool("Walking", false);
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsFalling", false);
        NoPlayer = true;
        Walking = false;
    }
    private void UpdatePath()
    {
        if (NoPlayer == false && Walking == true)
        {
            if (TargetInDistance() && seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
    }
    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);
        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();
        NoPlayer = false;
        return hit.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
    void UpdateAnimationState()
    {
        if (NoPlayer == false && Walking == true)
        {
            if (speedofwalking < 0f || speedofwalking > 0f)
            {
                anim.SetBool("Walking", true);
            }
            else
            {
                anim.SetBool("Walking", true);
            }
            if (rb.velocity.y > .1f)
            {
                anim.SetTrigger("IsJumping");
            }
            else if (rb.velocity.y < -.1f)
            {
                anim.SetTrigger("IsFalling");
            }
        }
    }
    private void PathFollow()
    {
        if (NoPlayer == false && Walking == true)
        {
            if (path == null)
            {
                return;
            }

            // Reached end of path
            if (currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }


            // Direction Calculation
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed;

            // Movement
            rb.velocity = new Vector2(force.x, rb.velocity.y);

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }


        }
    }
    private void Jump()
    {
            rb.velocity = new Vector3(rb.velocity.x, jumpModifier);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NoPlayer == false && Walking == true)
        {
            Jump();
        }
    }
    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (NoPlayer == false && Walking == true)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }
    }
    private bool Ground()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f,  jumpableGround);
    }
}
