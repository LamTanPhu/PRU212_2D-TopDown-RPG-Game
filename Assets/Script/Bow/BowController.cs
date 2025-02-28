using UnityEngine;

public class BowController : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Reference to the player object
    public Transform firePoint; // FirePoint where arrows are spawned
    public GameObject arrowPrefab; // Arrow prefab

    [Header("Bow Settings")]
    public float minArrowSpeed = 15f;
    public float maxArrowSpeed = 30f;
    public float chargeTime = 1f; // Time to fully charge the arrow
    public float bowDistance = 1.5f;
    public float shootCooldown = 0.2f; // Prevents rapid spam clicking

    private Animator animator;
    private float chargeAmount = 0f;
    private bool isCharging = false;
    private bool isFullyCharged = false;
    private bool canShoot = true;

    private enum BowState { Idle, Charging, FullyCharged, Shooting }
    private BowState currentState = BowState.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
        }

        SetState(BowState.Idle);
    }

    void Update()
    {
        MoveBowAroundPlayer();

        // Start charging when left mouse button is pressed
        if (Input.GetMouseButtonDown(0) && canShoot && currentState == BowState.Idle)
        {
            StartCharging();
        }

        // Increase charge while holding the button
        if (Input.GetMouseButton(0) && isCharging)
        {
            chargeAmount += Time.deltaTime;
            chargeAmount = Mathf.Clamp(chargeAmount, 0f, chargeTime);

            if (chargeAmount >= chargeTime && currentState != BowState.FullyCharged)
            {
                isFullyCharged = true;
                SetState(BowState.FullyCharged);
            }
        }

        // Fire when the button is released
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            FireArrow();
        }
    }

    /// <summary>
    /// Moves the bow around the player based on the mouse position.
    /// </summary>
    void MoveBowAroundPlayer()
    {
        if (player == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure it's in 2D space
        Vector2 direction = (mousePos - player.position).normalized;

        // Position the bow relative to the player
        transform.position = player.position + (Vector3)(direction * bowDistance);

        // Rotate the bow to face the cursor
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    /// <summary>
    /// Starts charging the bow.
    /// </summary>
    void StartCharging()
    {
        if (currentState != BowState.Idle) return;

        isCharging = true;
        chargeAmount = 0f;
        isFullyCharged = false;
        SetState(BowState.Charging);
    }

    /// <summary>
    /// Fires an arrow and plays the shooting animation.
    /// </summary>
    void FireArrow()
    {
        if (!canShoot || arrowPrefab == null || firePoint == null) return;

        canShoot = false;
        SetState(BowState.Shooting);

        // Instantiate the arrow
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        // Calculate arrow speed based on charge amount
        float arrowSpeed = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, chargeAmount / chargeTime);
        Vector2 shootDirection = firePoint.right;
        arrowRb.linearVelocity = shootDirection * arrowSpeed;

        // Set arrow rotation
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);

        // Ensure animation fully plays before resetting
        float shootAnimationTime = GetAnimationLength("Shoot");
        Invoke(nameof(ResetCharge), shootAnimationTime);
        Invoke(nameof(EnableShooting), shootAnimationTime + shootCooldown);
    }

    /// <summary>
    /// Resets the charge and bow state after shooting.
    /// </summary>
    void ResetCharge()
    {
        isCharging = false;
        chargeAmount = 0f;
        isFullyCharged = false;
        SetState(BowState.Idle);
    }

    /// <summary>
    /// Enables shooting again after the cooldown.
    /// </summary>
    void EnableShooting()
    {
        canShoot = true;
    }

    /// <summary>
    /// Changes the bow's animation state.
    /// </summary>
    void SetState(BowState newState)
    {
        currentState = newState;

        // Reset animator parameters
        animator.SetBool("IsCharging", false);
        animator.SetBool("IsFullyCharged", false);

        // Set the correct animation state
        switch (newState)
        {
            case BowState.Idle:
                animator.SetTrigger("BowIdle");
                break;
            case BowState.Charging:
                animator.SetBool("IsCharging", true);
                break;
            case BowState.FullyCharged:
                animator.SetBool("IsFullyCharged", true);
                break;
            case BowState.Shooting:
                animator.SetTrigger("Shoot");
                break;
        }
    }

    /// <summary>
    /// Gets the length of an animation state by name.
    /// </summary>
    float GetAnimationLength(string animationName)
    {
        if (animator == null) return shootCooldown;

        // Retrieve the animation state info
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the current state matches the requested animation
        if (stateInfo.IsName(animationName))
        {
            return stateInfo.length;
        }

        return shootCooldown; // Default to cooldown time if animation length can't be determined
    }
}