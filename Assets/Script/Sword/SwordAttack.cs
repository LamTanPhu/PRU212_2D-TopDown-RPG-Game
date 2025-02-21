using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Animator swordAnimator; // Assign in Inspector
    public Transform sword; // Assign the Sword GameObject (Make sure it's inactive at start)
    private PlayerMovement playerMovement;
    private bool isAttacking = false;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>(); // Find player movement script
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        sword.gameObject.SetActive(true); // Show the sword

        // Get the player's last movement direction
        Vector2 direction = playerMovement.lastIdleDirection;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Horizontal swing
        {
            swordAnimator.Play(direction.x > 0 ? "SwingRight" : "SwingLeft");
        }
        else // Vertical swing
        {
            swordAnimator.Play(direction.y > 0 ? "SwingUp" : "SwingDown");
        }

        yield return new WaitForSeconds(swordAnimator.GetCurrentAnimatorStateInfo(0).length); // Wait until animation finishes

        sword.gameObject.SetActive(false); // Hide the sword
        isAttacking = false;
    }
}
