using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minChangeTime = 2f;
    public float maxChangeTime = 5f;
    public float idleChance = 0.3f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;

        if (moveDirection == Vector2.zero)
        {
            animator.SetTrigger("Idle");  // Use trigger instead of Play()
        }
        else
        {
            animator.SetTrigger("Walk");
        }
    }

    IEnumerator ChangeDirectionRoutine()
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
        animator.SetTrigger("Idle");
    }

    public void ResumeMoving()
    {
        StartCoroutine(ChangeDirectionRoutine()); // Resume patrol after attack
    }

}
