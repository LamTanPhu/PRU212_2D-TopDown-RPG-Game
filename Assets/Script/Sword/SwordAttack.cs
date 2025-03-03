using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int attackDamage = 20; // Sát thương của kiếm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Nếu va chạm với quái
        {
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            BossAI boss = collision.GetComponent<BossAI>();
            if (enemy != null)
            {
                enemy.EnemyTakeDamage(attackDamage);
            }
        }
    }
}
