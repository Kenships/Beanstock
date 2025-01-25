using System;
using System.Collections;
using UnityEngine;

public class WallEnemy : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _hitBox;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackTime;
    [SerializeField] private float attackRecovery;
    private State _enemyState;
    [SerializeField] private Color attackColor;
    [SerializeField] private Color neutralColor;
    [SerializeField] private Color windupColor;
    [SerializeField] private float detectionRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hitBox = gameObject.GetComponent<BoxCollider2D>();
        _enemyState = State.Idle;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private enum State{
        Attacking,
        Idle,
        Recovering
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyState){
            case State.Idle:
                if(Vector3.Distance(transform.position, playerTransform.position) < detectionRange && _enemyState == State.Idle)
                    StartCoroutine(Attack());

                _hitBox.enabled = false;
                _spriteRenderer.color = neutralColor;
                break;
            case State.Attacking:
                _hitBox.enabled = true;
                _spriteRenderer.color = attackColor;
                break;
            case State.Recovering:
                _hitBox.enabled = false;
                _spriteRenderer.color = windupColor;
                break;
        }
    }

    IEnumerator Attack(){
        _enemyState = State.Recovering;
        yield return new WaitForSeconds(attackDelay);
        _enemyState = State.Attacking;
        yield return new WaitForSeconds(attackTime);
        //_enemyState = State.Recovering;
        //yield return new WaitForSeconds(attackRecovery);
        _enemyState = State.Idle;

    }
}
