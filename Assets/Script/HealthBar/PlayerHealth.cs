using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; } = false;

    private Animator animator;
    private PlayerMovement playerMovement;
    private Rigidbody2D rgb;

    private void Awake()
    {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        rgb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);
        FindObjectOfType<HealthBar>()?.UpdateHealthBar();

        if (CurrentHealth <= 0 && !IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // Stop movement and dashing
        playerMovement.enabled = false;
        playerMovement.isDashing = false;

        // Stop all movement
        rgb.linearVelocity = Vector2.zero;
        StopAllCoroutines(); // Ensure no movement continues

        // Stop movement animations
        animator.SetBool("isDashing", false);
        animator.SetFloat("AnimMagnitude", 0);
        animator.SetFloat("AnimMoveX", playerMovement.lastIdleDirection.x);
        animator.SetFloat("AnimMoveY", playerMovement.lastIdleDirection.y);

        // Play death animation
        animator.SetTrigger("Die");
    }

    private void Update()
    {
        if (CurrentHealth <= 0 && !IsDead)
        {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }
}