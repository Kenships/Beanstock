using System.Collections;
using System.Runtime.CompilerServices;
using DamageManagement;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float _health;
    private Vector2 originalPosition;
    private const float respawnTime = 2;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject respawn;
    private const float _flashTime = 0.075f;
    [SerializeField ]private Material normal;
    [SerializeField] private Material flash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = maxHealth;
        originalPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // if(other.gameObject.CompareTag("Player Attack")){
        //     _health--;
        //     if(_health <= 0){
        //         Die();
        //     }
        //     else{
        //         StartCoroutine(GetHit());
        //     }
        // }
    }

    private IEnumerator GetHit(){
        Color originalColor = sprite.color;
        sprite.material = flash;
        sprite.color = Color.white;
        yield return new WaitForSeconds(_flashTime);
        sprite.color = originalColor;
        sprite.material = normal;
    }

    public void Damage(float damage)
    {
        _health -= damage;
        if(_health <= 0){
            Die();
        }
        else{
            StartCoroutine(GetHit());
        }
    }

    public void Die()
    {
        _health = maxHealth;
        GameObject MyRespawn = Instantiate(respawn, originalPosition, Quaternion.identity);
        MyRespawn.GetComponent<RespawnHolder>().enemy = gameObject;
    }
    
}
