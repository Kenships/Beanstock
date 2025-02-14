using System.Collections;
using System.Runtime.CompilerServices;
using DamageManagement;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth;
    private float _health;
    private Vector2 originalPosition;
    private const float respawnTime = 2;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject respawn;
    private const float _flashTime = 0.3f;
    [SerializeField ]private Material normal;
    [SerializeField] private Material flash;
    private Rigidbody2D _rb;
    [SerializeField] private float hitSpeed;
    [SerializeField] private float hitDrag;
    private Color originalColor;
    private float _invinvible;

    private void Awake() {
        originalColor = sprite.color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = maxHealth;
        originalPosition = transform.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update(){
        _invinvible -= Time.deltaTime;
    }

    private void OnEnable() {
        sprite.color = originalColor;
        sprite.material = normal;
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

    IEnumerator GetHit()
    {
        Vector3 playerPosition;
        //knock back
        //transform.up = GetAimPosition(transform.position, playerPosition) * -1;
        //_rb.linearVelocity = transform.up * hitSpeed;
        //transform.up = Vector3.zero;
        //end of knockback

        sprite.material = flash;
        sprite.color = Color.white;

        for(float i = 0; i < _flashTime; i += Time.deltaTime){
            yield return null;
            _rb.linearVelocity *= 1 - Time.deltaTime * hitDrag;
        }
        //yield return new WaitForSeconds(_flashTime)

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

    private Vector3 GetAimPosition(Vector3 a, Vector3 b){
        return new Vector3(a.x - b.x, a.y - b.y) * -1;
    }

    public void Die(){
        _health = maxHealth;
        GameObject MyRespawn = Instantiate(respawn, originalPosition, Quaternion.identity);
        MyRespawn.GetComponent<RespawnHolder>().enemy = gameObject;
    }
    
}
