using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("StupidFu")]
    bool isFacingRight = true;
    public Rigidbody2D rb;
    public Animator anim;
    public BoxCollider2D coll;
    public BoxCollider2D slideColl;
    private SpriteRenderer sprite;
    public static PlayerMovement instance;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private TrailRenderer tr;

    [Header("Movement")]

    private float dirX;
    public float speed = 5f;
    public float jump = 10f;

    [Header("Dash")]

    private bool canDash = true;
    public bool isDashing;
    public float dashingPower = 24f;
    public float dasingTime = 0.2f;
    private float dashingCooldown = 0.5f;

    [Header("WallCheck")]

    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);

    [Header("WallMovement")]

    public float walllSlideSpeed = 2;
    bool isWallSliding;
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.17f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 14f);

    [Header("Attack")]

    /*  public bool bounced = false;
      public float bounce;*/
    public LayerMask enemyLayers;
    public float attackRange;
    public int Damage;
    public Transform attackPoint;
    public bool isAttacking = false;

    private enum MovementState { idle, running, jumping, falling }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
            if (!isWallJumping)
            {
            dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dirX * speed, rb.velocity.y);
            Flip();
            }
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, jump);
            }
            if (Input.GetButtonDown("Jump") && wallJumpTimer > 0f)
            {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            if(transform.localScale.x != wallJumpDirection)
            {
                            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime);
            }
        /*
       if(Time.time >= nextAttackTime)
   {
          if (Input.GetMouseButtonDown(0) && !IsGrounded())
       {
           DownAttack();
       }
       }*/
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        Attackin();
        ProcessWallSlide();
        ProcessWallJump();
        UpdateAnimationState();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }
    private void UpdateAnimationState()
    {
        MovementState state;
        if (dirX > 0f)
        {
            state = MovementState.running;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }
        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }
        anim.SetBool("CanDash", isDashing);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetInteger("state", (int)state);
    }

    void Attackin()
    {
         if(Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
        }
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(Damage);
        }
    }

   /* public void DownAttack()
    {
        anim.SetTrigger("DownAttack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        rb.gravityScale = 5.6f;
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(Damage);
            rb.velocity = new Vector2(rb.velocity.x, bounce);
            bounced = true;
        }
    }*/
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, jumpableGround);
    }
    private void ProcessWallSlide()
    {
        if (!IsGrounded() & WallCheck() & dirX != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -walllSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void ProcessWallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }
    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    private void Flip()
    {
        if(isFacingRight && dirX < 0 || !isFacingRight && dirX > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dasingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
