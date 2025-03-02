using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Collider2D attackZone;
    public Collider2D detectionZone; // Khu vực phát hiện Player
    private bool playerInRange = false;
    private bool playerDetected = false;
    public enum EnemyState { Idle, Walk, Attack, Hurt, Dead }
    private EnemyState currentState = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float idleTime = 2f;
    public float walkTime = 3f;
    public float attackRange = 1.5f;
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
        if (isDead) return; // Nếu Enemy đã chết, không làm gì cả

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Walk:
                UpdateWalk();
                break;
            case EnemyState.Attack:
                if (!isAttacking && (player == null || Vector2.Distance(transform.position, player.position) > attackRange))
                {
                    currentState = EnemyState.Idle;
                }
                break;
        }

        // Nếu Player vào tầm, Enemy Attack ngay lập tức
        if (!isHurt && canAttack && !isAttacking && player && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            StartCoroutine(HandleAttack());
        }
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
        yield return new WaitForSeconds(0.17f);

        canAttack = true;
        isAttacking = false;
        // Nếu Player rời khỏi tầm, quay về Idle ngay lập tức
        if (shouldStopAttack)
        {
            currentState = EnemyState.Idle;
        }
        yield return new WaitForSeconds(attackCooldown);

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
        Debug.Log("Enemy bị đánh! Máu còn: " + currentHealth);

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
                Debug.Log("Enemy bị đánh khi đang Attack - Dừng Attack ngay lập tức!");
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
            Debug.Log("👀 Player đã vào tầm tấn công!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("🏃 Player đã rời khỏi tầm tấn công!");
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