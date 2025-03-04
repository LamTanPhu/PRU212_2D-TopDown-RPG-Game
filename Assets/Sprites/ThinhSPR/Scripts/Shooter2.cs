using UnityEngine;
using System.Collections;

public class Shooter2 : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;         // Prefab with Projectile2 script
    [SerializeField] private float bulletSpeed = 22f;         // Base speed of the bullet
    [SerializeField] private float fireRate = 2f;            // Time between shots
    [SerializeField] private float homingDuration = 1f;       // Duration the bullet follows the player (seconds)
    [SerializeField] private float maxTurnSpeed = 180f;      // Maximum turn speed for homing (degrees per second)

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            return;
        }
        StartCoroutine(ShootBullets());
    }

    private IEnumerator ShootBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            if (player != null)
            {
                ShootAtPlayer();
            }
            else
            {
                Debug.LogWarning("Player lost during gameplay! Attempting to re-find...");
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (player == null)
                {
                    Debug.LogError("Player still not found! Stopping shooting.");
                    yield break;
                }
            }
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 targetPosition = player.position;
        Vector2 shooterPosition = transform.position;
        Vector2 direction = (targetPosition - shooterPosition).normalized;

        // Debug the positions and direction to verify targeting
        Debug.Log($"Shooting at Player - Player Pos: {targetPosition}, Shooter Pos: {shooterPosition}, Direction: {direction}, Magnitude: {direction.magnitude}");

        // Ensure direction isn’t zero (rare, but possible if positions are identical)
        if (direction.magnitude < 0.01f)
        {
            Debug.LogWarning("Direction is nearly zero! Defaulting to right for safety.");
            direction = Vector2.right; // Fallback to right if direction is invalid
        }

        // Adjust direction if scale is negative (fix flipped direction)
        if (transform.localScale.x < 0)
        {
            Debug.LogWarning("Enemy has negative X scale! Flipping direction.");
            direction = -direction; // Flip direction if scale is negative
        }

        // Instantiate a single bullet
        GameObject bullet = Instantiate(bulletPrefab, shooterPosition, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            // Set initial velocity in the direction of the player
            bulletRb.linearVelocity = direction * bulletSpeed;

            // Rotate the bullet to face the initial direction
            float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Start homing coroutine for the bullet
            Projectile2 projectile = bullet.GetComponent<Projectile2>();
            if (projectile != null)
            {
                projectile.StartHoming(player, homingDuration, maxTurnSpeed, bulletSpeed);
            }
            else
            {
                Debug.LogError("Bullet prefab is missing Projectile2 script!");
            }

            // Debug to confirm bullet spawning and direction
            Debug.Log($"Bullet spawned at: {bullet.transform.position}, Initial Velocity: {bulletRb.linearVelocity}, Direction: {direction}, Rotation: {bullet.transform.rotation.eulerAngles.z}, Target: {targetPosition}");

            // Ensure bullet is visible (check SpriteRenderer)
            SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning("Bullet prefab is missing a SpriteRenderer! Adding one.");
                spriteRenderer = bullet.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>("ArrowheadBullet"); // Replace with your sprite path or asset
                spriteRenderer.sortingLayerName = "Bullets"; // Set a visible sorting layer
                spriteRenderer.color = Color.white; // Ensure visible color
            }
            else
            {
                Debug.Log($"Bullet SpriteRenderer found, active: {spriteRenderer.enabled}, color: {spriteRenderer.color}");
            }
        }
        else
        {
            Debug.LogError("Bullet prefab is missing a Rigidbody2D!");
        }
    }
}