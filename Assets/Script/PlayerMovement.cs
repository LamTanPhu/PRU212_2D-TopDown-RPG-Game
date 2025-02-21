using System.Collections;
using Unity.VisualScripting;


using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float dashPower = 5f; // Controls dash distance
    public float dashDuration = 0.5f;
    public float dashCooldown = 0.5f; // Time before dashing again

    private Rigidbody2D rgb;
    private Animator animator;
    private Vector2 Movedirection;
    public Vector2 lastIdleDirection = Vector2.down;
    private bool isDashing = false;
    private float nextDashTime = 0f;

    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInputs();
        Animate();

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime&& Movedirection.magnitude>0)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Movedirection = new Vector2(moveX, moveY).normalized;

        if (Movedirection.magnitude > 0.1f)
        {
            lastIdleDirection = Movedirection;
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDirection = (mousePos - transform.position).normalized;

            if (lookDirection.magnitude > 0.1f) 
            {
                lastIdleDirection = lookDirection;
            }
        }
    }

    void Move()
    {
        rgb.linearVelocity = Movedirection * moveSpeed;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown; // Set cooldown

        Vector2 dashDirection = lastIdleDirection.normalized; // Ensure diagonal dashes
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rgb.linearVelocity = dashDirection * dashPower;
            yield return null;
        }

        isDashing = false;
        rgb.linearVelocity = Vector2.zero; // Stop moving after dash
    }

    void Animate()
    {
        animator.SetFloat("AnimMoveX", lastIdleDirection.x);
        animator.SetFloat("AnimMoveY", lastIdleDirection.y);
        animator.SetFloat("AnimMagnitude", Movedirection.magnitude);
        animator.SetBool("IsDashing", isDashing);
    }
}
