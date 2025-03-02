using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private int bulletCount = 6; // Number of bullets per attack
    [SerializeField] private float spreadAngle = 30f; // Spread of bullets

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
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 targetPosition = player.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        float startAngle = -spreadAngle / 2;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 bulletDirection = Quaternion.Euler(0, 0, angle) * direction;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletDirection * bulletSpeed;
            }
            else
            {
                Debug.LogError("Bullet prefab is missing a Rigidbody2D!");
            }
        }
    }
}
