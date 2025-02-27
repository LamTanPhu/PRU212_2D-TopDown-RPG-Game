using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float dashPower = 5f; // Dash speed
    public float dashDuration = 0.2f; // Shorter dash for a snappier feel
    public float dashCooldown = 0.5f; // Time before dashing again

    public GameObject crosshair; // Assign in Inspector

    private Rigidbody2D rgb;
    private Animator animator;
    private Vector2 moveDirection;
    public Vector2 lastIdleDirection = Vector2.down;
    private bool isDashing = false;
    private float nextDashTime = 0f;

    void Start()
    {
        Cursor.visible = false;
        rgb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInputs();
        Animate();
        UpdateCrosshairPosition();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextDashTime && moveDirection.magnitude > 0)
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

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            lastIdleDirection = moveDirection;
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
        rgb.linearVelocity = moveDirection * moveSpeed;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("isDashing", true); // Activate dash animation
        nextDashTime = Time.time + dashCooldown;
        Vector2 dashDirection = lastIdleDirection.normalized;
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rgb.linearVelocity = dashDirection * dashPower;
            yield return null;
        }
        float decelerationTime = 0.1f;
        float elapsed = 0f;
        Vector2 startVelocity = rgb.linearVelocity;
        while (elapsed < decelerationTime)
        {
            elapsed += Time.deltaTime;
            rgb.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, elapsed / decelerationTime);
            yield return null;
        }
        isDashing = false;
        animator.SetBool("isDashing", false);
        rgb.linearVelocity = Vector2.zero;
    }

    void Animate()
    {
        animator.SetBool("isDashing", isDashing);
        animator.SetFloat("AnimMoveX", lastIdleDirection.x);
        animator.SetFloat("AnimMoveY", lastIdleDirection.y);
        animator.SetFloat("AnimMagnitude", moveDirection.magnitude);
    }


    void UpdateCrosshairPosition()
    {
        if (crosshair != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            crosshair.transform.position = mousePosition;
        }
    }
}