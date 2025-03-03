using UnityEngine;

public class GravityBullet : MonoBehaviour
{
    private Vector2 targetPosition;
    private float pushForce;

    public void SetTarget(Vector2 holePosition, float force)
    {
        targetPosition = holePosition;
        pushForce = force;
    }

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0; // Disable default gravity
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 pushDirection = (targetPosition - (Vector2)collision.transform.position).normalized;
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
            Destroy(gameObject); // Bullet disappears after applying force
        }
    }
}
