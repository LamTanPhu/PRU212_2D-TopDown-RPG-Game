using UnityEngine;

public class HoleDamage : MonoBehaviour
{
    public int damage = 2; // Lượng sát thương gây ra
    public float damageCooldown = 1.5f; // Thời gian giữa các lần nhận sát thương
    private bool playerInHole = false; // Kiểm tra nếu player đang trong hố
    private float lastDamageTime; // Lưu thời gian lần cuối nhận sát thương
    public PlayerHealth playerHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHole = true;
            DealDamage(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInHole && Time.time > lastDamageTime + damageCooldown)
            {
                DealDamage(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHole = false;
        }
    }

    private void DealDamage(Collider2D other)
    {
        Debug.Log("Run here");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            lastDamageTime = Time.time;
            Debug.Log("💀 Player took " + damage + " damage from Hole!");
        }
    }
}
