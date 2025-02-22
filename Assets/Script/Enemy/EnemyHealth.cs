using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private Animator animator;
    private EnemyMovement enemyMovement;
    private bool isHurt = false; // Prevents multiple hits interfering with animation

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>(); // Reference movement
    }

    public void TakeDamage(int damage)
    {
        if (isHurt) return; // Prevent multiple hits at the same time

        currentHealth -= damage;
        isHurt = true;  // Mark as hurt
        animator.Play("Hurt");  // Play Hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(ResumeMovement), 0.2f); // Delay before resuming movement
        }
    }

    void ResumeMovement()
    {
        isHurt = false; // Reset hurt state
        animator.Play("Walk"); // Resume normal animation
    }

    void Die()
    {
        animator.Play("Die");
        enemyMovement.enabled = false; // Stop movement
        Destroy(gameObject, 1f); // Destroy after animation
    }
}
