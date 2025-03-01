using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minChangeTime = 1f;
    public float maxChangeTime = 3f;
    public float idleChance = 0.3f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Animator animator;

    public bool isHurt = false;
    public bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void FixedUpdate()
    {
        if (isAttacking || isHurt) return;

        rb.linearVelocity = moveDirection * moveSpeed; // Sửa lại thành velocity thay vì linearVelocity (lỗi cú pháp)

        if (moveDirection == Vector2.zero)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetTrigger("Idle");
            }
        }
        else
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetTrigger("Walk");
            }
        }
    }


    public IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minChangeTime, maxChangeTime);
            yield return new WaitForSeconds(waitTime);

            if (Random.value < idleChance)
            {
                moveDirection = Vector2.zero;
            }
            else
            {
                float angle = Random.Range(0f, 360f);
                moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            }
        }
    }

    public void StopMoving()
    {
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        isAttacking = true;
        //animator.SetTrigger("Idle");
    }

    public void ResumeMoving()
    {
        // Chuyển về animation phù hợp sau khi Hurt kết thúc
        if (moveDirection == Vector2.zero)
        {
            animator.SetTrigger("Idle");
        }
        else
        {
            animator.SetTrigger("Walk");
        }
        StartCoroutine(ChangeDirectionRoutine()); // Tiếp tục tuần tra
    }
}
