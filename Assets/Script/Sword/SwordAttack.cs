using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int attackDamage = 20; // Sát thương của kiếm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Nếu va chạm với quái
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}
