using UnityEngine;

public class EnemyGravityBullet : MonoBehaviour
{
    [SerializeField] private float healthThreshold = 50f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float bulletSpeed = 5f;        // Speed of the bullet moving toward the player
    [SerializeField] private float bulletStopDistance = 1f; // Distance to stop and stay near the player
    [SerializeField] private float detectionRadius = 2f;    // Radius to detect and start pushing
    [SerializeField] private float pushForce = 200f;        // Force to push the player
    [SerializeField] private GameObject gravityBulletPrefab;
    [SerializeField] private bool shootImmediately = true;

    private GameObject player;
    private bool hasFired = false;
    private GameObject activeBullet;
    private bool isPushing = false;
    private GameObject targetHole;                           // Store the closest hole

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure it has 'Player' tag.");
            return;
        }

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogError("Player has no Rigidbody2D!");
        }
        else
        {
            Debug.Log($"Player Rigidbody2D found - Mass: {playerRb.mass}, Gravity Scale: {playerRb.gravityScale}, Constraints: {playerRb.constraints}, Body Type: {playerRb.bodyType}");
            // Ensure FreezeRotation is set by default
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (shootImmediately)
        {
            FireGravityBullet();
            hasFired = true;
        }
    }

    void Update()
    {
        if (currentHealth <= healthThreshold && !hasFired && !shootImmediately)
        {
            FireGravityBullet();
            hasFired = true;
        }

        if (activeBullet != null)
        {
            StayAndPushPlayer();
        }

        // Log bullet state for debugging
        if (activeBullet != null)
        {
            Rigidbody2D bulletRb = activeBullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                Debug.Log($"Bullet State - Position: {activeBullet.transform.position}, Velocity: {bulletRb.linearVelocity}, Gravity Scale: {bulletRb.gravityScale}, Constraints: {bulletRb.constraints}");
            }
        }

        // Log player state during push for debugging
        if (player != null && player.GetComponent<Rigidbody2D>() != null && isPushing)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Debug.Log($"Player State during push - Position: {player.transform.position}, Velocity: {playerRb.linearVelocity}, Constraints: {playerRb.constraints}, Is Kinematic: {playerRb.isKinematic}");
        }
    }

    void FireGravityBullet()
    {
        activeBullet = Instantiate(gravityBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = activeBullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null)
        {
            Debug.LogError("Gravity Bullet prefab needs a Rigidbody2D!");
        }
        else
        {
            // Ensure bullet has no gravity and no constraints
            bulletRb.gravityScale = 0f;
            bulletRb.constraints = RigidbodyConstraints2D.None;
        }
        Debug.Log("Gravity Bullet fired at: " + activeBullet.transform.position);
    }

    void StayAndPushPlayer()
    {
        Rigidbody2D bulletRb = activeBullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null) return;

        if (player == null)
        {
            Debug.LogError("Player is null! Destroying bullet.");
            Destroy(activeBullet);
            activeBullet = null;
            return;
        }

        // Stay near the player at bulletStopDistance
        Vector2 playerPos2D = player.transform.position;
        Vector2 bulletPos2D = activeBullet.transform.position;
        float distanceToPlayer = Vector2.Distance(bulletPos2D, playerPos2D);

        if (distanceToPlayer > bulletStopDistance)
        {
            // Move toward the player if too far
            Vector2 directionToPlayer = (playerPos2D - bulletPos2D).normalized;
            bulletRb.linearVelocity = directionToPlayer * bulletSpeed;
        }
        else
        {
            // Stop and stay in place, ensuring no residual velocity or forces
            bulletRb.linearVelocity = Vector2.zero;
            bulletRb.angularVelocity = 0f; // Ensure no rotation
            bulletRb.WakeUp(); // Ensure the Rigidbody is awake and responsive
            Debug.Log("Bullet stopped and staying at: " + bulletPos2D + " | Distance to player: " + distanceToPlayer);

            // Check if within detection radius to push and player isn�t on the hole
            if (distanceToPlayer <= detectionRadius && !IsPlayerOnHole())
            {
                StartOrContinuePushing();
            }
            else if (IsPlayerOnHole())
            {
                StopPushing();
            }
        }
    }

    void StartOrContinuePushing()
    {
        if (!isPushing)
        {
            isPushing = true;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerRb != null && playerMovement != null)
            {
                Debug.Log("Starting push toward nearest hole");

                // Ensure no constraints prevent movement (but keep FreezeRotation)
                if (playerRb.constraints != RigidbodyConstraints2D.FreezeRotation)
                {
                    Debug.LogWarning("Player Rigidbody2D has unexpected constraints: " + playerRb.constraints);
                    playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }

                if (playerRb.bodyType == RigidbodyType2D.Kinematic)
                {
                    Debug.LogError("Player Rigidbody2D is Kinematic! Changing to Dynamic.");
                    playerRb.bodyType = RigidbodyType2D.Dynamic;
                }

                playerMovement.SetBeingPushed(true);
                playerRb.linearVelocity = Vector2.zero;

                // Find and set the closest random hole
                targetHole = FindNearestHole();
                if (targetHole == null)
                {
                    Debug.LogWarning("No holes found! Cannot push.");
                    isPushing = false;
                    return;
                }
            }
        }

        // Apply continuous push toward the target hole
        if (targetHole != null)
        {
            Vector2 playerPos2D = player.transform.position;
            Vector2 holePos2D = targetHole.transform.position;
            Vector2 pushDirection = (holePos2D - playerPos2D).normalized;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 force = pushDirection * (pushForce * Time.deltaTime);
                playerRb.AddForce(force, ForceMode2D.Force);

                // Prevent sticking by ensuring velocity isn�t zero
                if (playerRb.linearVelocity.magnitude < 0.1f && !IsPlayerOnHole())
                {
                    Debug.LogWarning("Player velocity too low - adding extra force to prevent sticking");
                    playerRb.AddForce(pushDirection * (pushForce * 2f * Time.deltaTime), ForceMode2D.Force);
                }
            }
        }
    }

    void StopPushing()
    {
        if (isPushing)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerRb != null && playerMovement != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerMovement.SetBeingPushed(false);
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            isPushing = false;
            Debug.Log("Push stopped - Player stepped on the hole");
            // Keep the bullet in place instead of destroying it
            Rigidbody2D bulletRb = activeBullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = Vector2.zero;
                bulletRb.angularVelocity = 0f;
            }
        }
    }

    private GameObject FindNearestHole()
    {
        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        if (holes.Length == 0)
        {
            Debug.LogWarning("No objects with 'Hole' tag found!");
            return null;
        }

        // Find the closest hole to the player
        GameObject nearestHole = null;
        float minDistance = float.MaxValue;
        Vector2 playerPos2D = player.transform.position;

        foreach (GameObject hole in holes)
        {
            Vector2 holePos2D = hole.transform.position;
            float distance = Vector2.Distance(playerPos2D, holePos2D);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestHole = hole;
            }
        }

        return nearestHole;
    }

    private bool IsPlayerOnHole()
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");

        if (playerCollider != null && holes != null)
        {
            foreach (GameObject hole in holes)
            {
                Collider2D holeCollider = hole.GetComponent<Collider2D>();
                if (holeCollider != null)
                {
                    // Check if player is touching or standing on the hole
                    if (playerCollider.IsTouching(holeCollider))
                    {
                        // Additional check for standing on top (y-position comparison)
                        Vector2 playerPos2D = player.transform.position;
                        Vector2 holePos2D = hole.transform.position;
                        float yDifference = playerPos2D.y - holePos2D.y;

                        // If player is above or on the hole (within a small threshold) and touching, consider it on the hole
                        if (yDifference >= -0.1f && yDifference <= 0.1f) // Adjust threshold as needed
                        {
                            Debug.Log("Player is standing on the hole!");
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        if (activeBullet != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)activeBullet.transform.position, bulletStopDistance); // Visualize where the bullet stays
        }
    }
}