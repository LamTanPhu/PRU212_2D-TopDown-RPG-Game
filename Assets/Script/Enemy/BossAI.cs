using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Collider2D attackZone;
    public bool playerInRange = false;
    public enum EnemyState { Idle, Walk, Attack, Hurt, Dead }
    private EnemyState currentState = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float idleTime = 2f;
    public float walkTime = 3f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int maxHealth = 100;
    public int attackDamage = 1;

    private int currentHealth;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool canAttack = true;
    private bool isHurt = false;
    private bool isAttacking = false; // Kiểm soát Attack không bị cắt ngang
    private bool isDead = false; // Kiểm soát Enemy có chết chưa
    private float stateTimer = 0f;

    public float chaseSpeed = 3f; // Tốc độ khi đuổi theo Player
    public float chaseRange = 5f; // Khoảng cách để bắt đầu đuổi theo Player

    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"));
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && canAttack && !isAttacking && !isHurt)
        {
            StartCoroutine(HandleAttack());
        }
        else if (distanceToPlayer <= chaseRange && !isAttacking && !isHurt)
        {
            MoveTowardsPlayer(); // 🟢 Đuổi theo Player khi trong phạm vi
        }
        else
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;
                case EnemyState.Walk:
                    UpdateWalk();
                    break;
            }
        }

        // Nếu Player vào tầm, Enemy Attack ngay lập tức
        if (!isHurt && canAttack && !isAttacking && player && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            StartCoroutine(HandleAttack());
        }
    }

    void MoveTowardsPlayer()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            animator.Play("Walk"); // ✅ Giữ nguyên hệ thống Animation

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
    }

    void UpdateIdle()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            animator.Play("Idle");

        stateTimer += Time.deltaTime;
        if (stateTimer >= idleTime)
        {
            moveDirection = Random.insideUnitCircle.normalized;
            currentState = EnemyState.Walk;
            stateTimer = 0f;
        }
    }

    void UpdateWalk()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            animator.Play("Walk");

        rb.linearVelocity = moveDirection * moveSpeed;
        stateTimer += Time.deltaTime;

        if (stateTimer >= walkTime)
        {
            rb.linearVelocity = Vector2.zero;
            currentState = EnemyState.Idle;
            stateTimer = 0f;
        }
    }

    IEnumerator HandleAttack()
    {
        isAttacking = true;
        currentState = EnemyState.Attack;
        canAttack = false;
        animator.Play("Attack");
        rb.linearVelocity = Vector2.zero;

        bool shouldStopAttack = false;

        // Kiểm tra nếu Player rời khỏi tầm khi Attack đang diễn ra
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1) // Khi animation Attack hoàn thành 1 lần
            {
                // Nếu Player đã rời khỏi tầm, đặt cờ dừng Attack sau khi hoàn thành
                if (player == null || Vector2.Distance(transform.position, player.position) > attackRange)
                {
                    shouldStopAttack = true;
                    break;
                }
            }

            yield return null; // Chờ mỗi frame
        }

        // Đợi Attack kết thúc hoàn toàn trước khi dừng
        yield return new WaitForSeconds(0.1f);

        canAttack = true;
        isAttacking = false;
        // Nếu Player rời khỏi tầm, quay về Idle ngay lập tức
        if (shouldStopAttack)
        {
            currentState = EnemyState.Idle;
        }

    }


    IEnumerator HandleHurt()
    {
        isHurt = true;
        canAttack = false;
        animator.Play("Hurt");
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f); // Đợi animation Hurt chạy hết

        isHurt = false;
        canAttack = true;

        // Nếu Player vẫn trong tầm, tiếp tục Attack sau Hurt
        if (player && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attack;
            StartCoroutine(HandleAttack());
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    void HandleDeath()
    {
        if (isDead) return; // Nếu đã chết, không làm gì thêm
        isDead = true;
        StopAllCoroutines(); // Dừng tất cả hành vi của Enemy
        animator.Play("Die");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void EnemyTakeDamage(int damage)
    {
        if (isDead || currentState == EnemyState.Hurt) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentState = EnemyState.Dead;
            HandleDeath();
        }
        else
        {
            StopAllCoroutines();

            if (currentState == EnemyState.Attack)
            {
                isAttacking = false;
                canAttack = false;
            }

            StartCoroutine(HandleHurt());
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Hàm này được gọi từ Animation Event khi đòn tấn công xảy ra
    public void DealDamageToPlayer()
    {
        if (playerInRange && playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("💥 Enemy đã gây sát thương!");
        }
    }

}
