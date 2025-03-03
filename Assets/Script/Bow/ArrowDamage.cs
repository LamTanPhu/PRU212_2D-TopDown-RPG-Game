using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public int damage = 20; // Arrow damage value
    public float lifetime = 3f; // How long the arrow exists before disappearing

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy arrow after some time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // If the arrow hits an enemy
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.EnemyTakeDamage(damage); // Deal damage to the enemy
                Debug.Log("Arrow hit enemy! Dealt " + damage + " damage.");
            }

            Destroy(gameObject); // Destroy the arrow after hitting
        }
        else if (collision.CompareTag("Wall")) // Arrow stops when hitting a wall
        {
            Destroy(gameObject);
        }
    }
}
