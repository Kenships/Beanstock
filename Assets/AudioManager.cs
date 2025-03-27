using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Assertions; 

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private AudioSource audioSource;
    [Header("Audio Sources")] 
    [SerializeField] AudioSource MusicSource;
    //[SerializeField] AudioSource SFXSource;

    [Header("SFX Clips")] 
    public AudioClip trapSound;
    public AudioSource walkSounds;
    public AudioClip enemyHit; // When enemy gets hit
    public AudioClip playerHit; // When the player gets hit
    public AudioClip playerDash;
    public AudioClip playerJump;
    public AudioSource zipSound;
    public AudioClip wallJump;
    public AudioClip bossSmash; 
    public AudioClip whoosh;
    public AudioClip bossRoar; 
    public AudioClip checkPoint;
    public AudioClip playerDeath;
    public AudioClip respawn;
    public AudioClip mageSpill;
    
    [Header("Music Clips")]
    public AudioClip calmBackground;
    public AudioClip bossTheme;
    public AudioClip runningTheme;
    public AudioClip ending;  
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        setMusic(runningTheme);
    }

    public void setMusic(AudioClip music){
        MusicSource.clip = music;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource audio = Instantiate(audioSource, transform.position, Quaternion.identity).GetComponent<AudioSource>();
        audio.clip = clip;
        audio.Play();
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        AudioSource audio = Instantiate(audioSource, transform.position, Quaternion.identity).GetComponent<AudioSource>();
        audio.clip = clip;
        audio.volume = volume;
        audio.Play();
    }

    public void PlaySFX(AudioClip clip, Vector3 position){
        if(Vector3.Distance(position, playerPosition.position) < 30){
            PlaySFX(clip);
        }
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume){
        if(Vector3.Distance(position, playerPosition.position) < 30){
            PlaySFX(clip, volume);
        }
    }

    public void PlayFootsteps()
    {
        walkSounds.volume = 0.4f;
    }

    public void StopFootsteps()
    {
        walkSounds.volume = 0;
    }

    public void PlayZip(){
        zipSound.volume = 0.6f;
    }

    public void StopZip(){
        zipSound.volume = 0;
    }
}
