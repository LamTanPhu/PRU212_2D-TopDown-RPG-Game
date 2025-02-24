using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 3f;
    public int attackDamage = 1;
    private bool isPerformingAttack = false;

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
        if (player != null && !enemyMovement.isAttacking && !isPerformingAttack)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

        public IEnumerator PerformAttack()
        {
            enemyMovement.isAttacking = true;
            StopCoroutine(enemyMovement.ChangeDirectionRoutine()); // Ngăn đổi hướng khi tấn công
            enemyMovement.StopMoving(); 
            animator.Play("Attack");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetTrigger("Idle");
            yield return new WaitForSeconds(attackCooldown);
            enemyMovement.isAttacking = false;
            enemyMovement.ResumeMoving();
        }

        public void StopAttacking()
        {
            StopAllCoroutines();
            enemyMovement.isAttacking = false;
            isPerformingAttack = false;

            animator.Play("Idle"); // Đặt Idle để tránh bị kẹt
        }

    public void PlayerTakeDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Take Damage");
        }
    }
}
