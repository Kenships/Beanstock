using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("SFX Clips")]
    public AudioClip trapSound;
    public AudioClip walkSounds;
    public AudioClip enemyHit; // When enemy gets hit
    public AudioClip playerHit; // When the player gets hit
    
    [Header("Music Clips")]
    public AudioClip calmBackground;
    public AudioClip bossTheme;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = calmBackground;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayFootsteps()
    {
        // Only start footsteps if they're not already playing.
        if (SFXSource.clip == walkSounds && SFXSource.isPlaying)
            return;
        
        SFXSource.clip = walkSounds;
        SFXSource.loop = true;
        SFXSource.Play();
    }

    public void StopFootsteps()
    {
        // Only stop if the currently playing clip is the footsteps.
        if (SFXSource.clip == walkSounds && SFXSource.isPlaying)
        {
            SFXSource.Stop();
            // Optionally reset the clip and looping state so other SFX won't be affected.
            SFXSource.clip = null;
            SFXSource.loop = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
