using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;          // Base speed of the projectile
    [SerializeField] private bool isEnemyProjectile = false;  // Whether this is an enemy projectile
    [SerializeField] private float projectileRange = 10f;     // Maximum distance the projectile can travel

    private Vector3 startPosition;                           // Starting position to track range
    private PlayerHealth playerHealth;                       // Reference to player’s health
    private Rigidbody2D rb;                                  // Reference to Rigidbody2D for physics movement
    private Transform target;                                // Target (player) for homing
    private float homingDuration;                            // Duration of homing behavior
    private float maxTurnSpeed;                              // Maximum turn speed for homing
    private float baseSpeed;                                 // Store base speed for consistency
    private bool isHoming = false;                           // Track homing state
    private float homingTimer;                               // Timer for homing duration

    private void Start()
    {
        startPosition = transform.position;
        playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Projectile2 is missing a Rigidbody2D!");
            return;
        }

        // Ensure bullet is visible
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Projectile2 is missing a SpriteRenderer! Adding one.");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("ArrowheadBullet"); // Replace with your sprite path or asset
            spriteRenderer.sortingLayerName = "Bullets"; // Set a visible sorting layer
        }
        else
        {
            Debug.Log($"Projectile2 SpriteRenderer found, active: {spriteRenderer.enabled}, color: {spriteRenderer.color}");
        }

        // Debug initial state
        Debug.Log($"Projectile2 started at: {transform.position}, Initial Velocity: {rb?.linearVelocity}, moveSpeed: {moveSpeed}");
    }

    public void StartHoming(Transform playerTarget, float duration, float turnSpeed, float speed)
    {
        target = playerTarget;
        homingDuration = duration;
        maxTurnSpeed = turnSpeed;
        baseSpeed = speed;
        isHoming = true;
        homingTimer = 0f;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();

        // Debug bullet state
        Debug.Log($"Projectile2 Update - Position: {transform.position}, Velocity: {rb?.linearVelocity}, IsHoming: {isHoming}, Target: {target?.position}");
    }

    private void MoveProjectile()
    {
        if (rb == null) return;

        if (isHoming && target != null)
        {
            homingTimer += Time.deltaTime;
            if (homingTimer < homingDuration)
            {
                // Homing behavior: steer toward the player
                Vector2 targetPosition = target.position;
                Vector2 currentPosition = transform.position;
                Vector2 desiredDirection = (targetPosition - currentPosition).normalized;

                // Calculate current direction from velocity
                Vector2 currentDirection = rb.linearVelocity.normalized;
                if (currentDirection == Vector2.zero)
                {
                    currentDirection = desiredDirection; // Fallback if velocity is zero
                }

                // Calculate the angle to rotate toward the target
                float angleToTarget = Vector2.SignedAngle(currentDirection, desiredDirection);
                float turnAmount = Mathf.Clamp(angleToTarget, -maxTurnSpeed * Time.deltaTime, maxTurnSpeed * Time.deltaTime);

                // Rotate the direction
                Quaternion rotation = Quaternion.Euler(0, 0, turnAmount);
                Vector2 newDirection = rotation * currentDirection;

                // Set velocity toward the target at base speed
                rb.linearVelocity = newDirection * baseSpeed;

                // Rotate the bullet to face the direction of movement
                float rotationAngle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
            else
            {
                // Switch to straight-line movement after homing duration
                isHoming = false;
                Vector2 finalDirection = rb.linearVelocity.normalized;
                if (finalDirection != Vector2.zero)
                {
                    rb.linearVelocity = finalDirection * moveSpeed;
                }
                else
                {
                    Debug.LogWarning("Final direction is zero! Defaulting to right.");
                    rb.linearVelocity = Vector2.right * moveSpeed;
                }
            }
        }
        else
        {
            // Straight-line movement after homing or if no target
            if (rb.linearVelocity.magnitude < 0.1f) // Prevent stalling
            {
                Vector2 currentDirection = rb.linearVelocity.normalized;
                if (currentDirection == Vector2.zero)
                {
                    Debug.LogWarning("Velocity is zero! Defaulting to right.");
                    currentDirection = Vector2.right;
                }
                rb.linearVelocity = currentDirection * moveSpeed;
            }

            // Rotate to match the velocity direction for arrowhead visualization
            Vector2 velocity = rb.linearVelocity;
            if (velocity != Vector2.zero)
            {
                float rotationAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }
        }

        // Debug movement
        Debug.Log($"Projectile2 Move - Velocity: {rb.linearVelocity}, Position: {transform.position}, Rotation: {transform.rotation.eulerAngles.z}, IsHoming: {isHoming}");
    }

    private void DetectFireDistance()
    {
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Debug.Log("Projectile destroyed due to exceeding range.");
            Destroy(gameObject); // Destroy if range exceeded
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!");

            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1); // Damage the player
            }
            Destroy(gameObject); // Destroy on hit
            Debug.Log("Projectile destroyed after hitting player.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Projectile2 destroyed at: " + transform.position);
    }
}