﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; } = false;

    private Animator animator;
    private PlayerMovement playerMovement;
    private Rigidbody2D rgb;
    AudioManager audioManager;

    private void Awake()
    {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        rgb = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        audioManager.PlaySFX(audioManager.playerHurt);
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
        audioManager.PlaySFX(audioManager.playerDie);
        animator.SetTrigger("Die");
        StartCoroutine(BacktoScreen());
    }

    IEnumerator BacktoScreen()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main Menu");
    }

    private void Update()
    {
        if (CurrentHealth <= 0 && !IsDead)
        {
            Die();
        }

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    TakeDamage(1);
        //}
    }
}