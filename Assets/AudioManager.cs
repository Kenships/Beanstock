using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;
    private AudioSource _damageSource; // Plays when enemy gets hit
    private AudioSource _enemyDamageSource;

    [Header("SFX Clips")]
    public AudioClip trapSound;
    public AudioClip[] walkSounds;
    public AudioClip enemyHit; // When enemy gets hit
    public AudioClip playerHit; // When the player gets hit
    public AudioClip playerDash;
    
    [Header("Music Clips")]
    public AudioClip calmBackground;
    public AudioClip bossTheme;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicSource.clip = calmBackground;
        MusicSource.Play();
    }

    public void playEnemyHitSound()
    {
        // If it is already playing return
        if (_damageSource.clip == enemyHit && _damageSource.isPlaying)
            return;

        _damageSource.clip = enemyHit;
        _damageSource.Play();
    }
    
    public void playPlayerHitSound()
    {
        // If it is already playing return
        if (_damageSource.clip == playerHit && _damageSource.isPlaying)
            return;

        _damageSource.clip = playerHit;
        _damageSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayFootsteps()
    {
        // Assert the length is not 0 to avoid out of bound errors
        Assert.IsTrue(walkSounds.Length > 0);
        
        // Only start footsteps if they're not already playing.
        if (walkSounds.Contains(SFXSource.clip) && SFXSource.isPlaying)
            return;
        
        SFXSource.clip = walkSounds[Random.Range(0, walkSounds.Length)];
        SFXSource.loop = true;
        SFXSource.Play();
    }

    public void StopFootsteps()
    {
        // Only stop if the currently playing clip is the footsteps.
        if (walkSounds.Contains(SFXSource.clip) && SFXSource.isPlaying)
        {
            SFXSource.Stop();
            // Optionally reset the clip and looping state so other SFX won't be affected.
            SFXSource.clip = null;
            SFXSource.loop = false;
        }
    }
}
