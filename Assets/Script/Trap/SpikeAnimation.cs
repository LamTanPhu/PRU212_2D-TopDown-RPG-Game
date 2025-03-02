using UnityEngine;

public class Spike : MonoBehaviour
{
    private Animator animator;
    public PlayerHealth playerHealth;
    private bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.Play("Spike");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInRange = false;
    }

    public void DealDamageToPlayer()
    {
        if (playerInRange && playerHealth != null)
        {
            playerHealth.TakeDamage(1);
        }
    }
}
