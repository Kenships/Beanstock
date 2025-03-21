using UnityEngine;

public class BossStart : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    private AudioManager _audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss.SetActive(false);
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        boss.SetActive(true);
        _audioManager.setMusic(_audioManager.bossTheme);
        Destroy(gameObject);
    }
}
