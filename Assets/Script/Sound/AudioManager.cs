using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance

    [Header("Audio Sources")]
    public AudioSource musicSource;  // Background Music
    public AudioSource sfxSource;    // Sound Effects
    public AudioSource spikeSfxSource;

    [Header("Audio Clips")]
    public AudioClip background; // Assign in Inspector
    public AudioClip swordSwing;
    public AudioClip bowShoting;
    public AudioClip playerHurt;
    public AudioClip playerDie;
    public AudioClip skeletonDie;
    public AudioClip DeathBringerDie;
    public AudioClip GolemDie;
    public AudioClip enemyHit;
    public AudioClip chestOpen;
    public AudioClip playerDash;
    public AudioClip spike;
    public AudioClip skeletonAttack;
    public AudioClip DeathBringerAttack;
    public AudioClip GolemAttack;
    public AudioClip BossDie;


    private void Start()
    {
        PlayMusic(background); // Start background music
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySpikeSFX(AudioClip clip)
    {
        if (spikeSfxSource != null && clip != null)
        {
            spikeSfxSource.PlayOneShot(clip);
        }
    }
}
