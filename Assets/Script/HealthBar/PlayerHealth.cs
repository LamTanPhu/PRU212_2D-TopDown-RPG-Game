using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    public float CurrentHealth { get; private set; }
    private bool isDead = false;

    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void TakeDamage(float _damage)
    {
        if (isDead) return;
        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, StartingHealth);
        Debug.Log($"Health changed from {previousHealth} to {CurrentHealth}");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Set death animation
        animator.SetFloat("AnimMoveX", playerMovement.lastIdleDirection.x);
        animator.SetFloat("AnimMoveY", playerMovement.lastIdleDirection.y);
        animator.SetTrigger("Die");

        // Disable movement
        playerMovement.enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // ✅ Disable all weapon GameObjects in child objects
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E Pressed");
            TakeDamage(1);
        }
    }

}