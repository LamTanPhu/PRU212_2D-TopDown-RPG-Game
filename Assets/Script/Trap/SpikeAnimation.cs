﻿using UnityEngine;

public class Spike : MonoBehaviour
{
    private Animator animator;
    public PlayerHealth playerHealth;
    private bool playerInRange;
    AudioManager audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = gameObject.GetComponent<Animator>();
        animator.Play("Spike");
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

    public void DealDamageToPlayer()
    {
        if (playerInRange && playerHealth != null)
        {
            playerHealth.TakeDamage(1);
        }
    }

    public void SpikeSound()
    {
        audioManager.PlaySpikeSFX(audioManager.spike);
    }
}
