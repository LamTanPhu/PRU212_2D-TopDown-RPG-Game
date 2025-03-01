using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private EnemyMovement enemyMovement;
    private EnemyAttack enemyAttack;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    public void TakeDamage(int damage)
    {
        //if (enemyMovement.isHurt) return; // Ngăn bị đánh trùng
        currentHealth -= damage;
        enemyMovement.isHurt = true;
        enemyMovement.StopMoving();
        if (enemyMovement.isAttacking)
        {
            enemyAttack.StopAttacking();
        }
        animator.CrossFade("Hurt", 0.1f);
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Chờ đúng thời gian của animation Hurt trước khi tiếp tục
            float hurtAnimTime = 0.35f;
            Invoke(nameof(ResumeMovement), hurtAnimTime);
        }
    }

    void ResumeMovement()
    {
        enemyMovement.isHurt = false;
        enemyMovement.ResumeMoving(); // Tiếp tục di chuyển

    }

    void Die()
    {
        enemyMovement.StopMoving(); // Dừng di chuyển khi chết
        animator.Play("Die");

        // Chờ animation chết chạy hết rồi dừng lại ở frame cuối
        float dieAnimTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(FinalizeDeath), dieAnimTime);
    }

    void FinalizeDeath()
    {
        animator.enabled = false; // Freeze animation ở frame cuối
        Destroy(gameObject, 0f); // Xóa ngay lập tức
    }
}
