using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;
    private bool isAttacking = false;

    private Transform player;
    private Animator animator;
    private EnemyMovement enemyMovement;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        enemyMovement.StopMoving();
        animator.Play("Attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Wait for attack animation to finish

        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            //player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            Debug.Log("Take Damage");
        }

        animator.Play("Idle"); // Manually switch back to Idle
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        enemyMovement.ResumeMoving();
    }
}
