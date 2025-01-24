using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Vector2 originalPosition;
    private const float respawnTime = 2;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject respawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player Attack"){
            Die();
        }
    }

    void Die(){
        GameObject MyRespawn = Instantiate(respawn, originalPosition, Quaternion.identity);
        MyRespawn.GetComponent<RespawnHolder>().enemy = gameObject;
    }
}
