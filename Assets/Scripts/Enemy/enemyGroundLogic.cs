using System;
using UnityEngine;

public class enemyGroundLogic : MonoBehaviour
{
    public Transform _chaseTarget;
    private Rigidbody2D _rb;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _jumpSpeed;
    private float _aimDirection;
    private bool _onGround;
    private bool _onWall;
    private State _enemyState;
    public Vector3[] _patrolPosition;
    private int _patrolNum;
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _chaseRange;
    private Vector3 _originalPosition;
    [SerializeField] private float _moveRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalPosition = transform.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private enum State{
        Patrolling,
        Attacking,
        Chasing
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyState){
            case State.Patrolling:

                Move(_patrolPosition[_patrolNum]);

                if(Vector3.Distance(transform.position, _patrolPosition[_patrolNum]) < 1){
                    _patrolNum ++;
                    if(_patrolNum >= _patrolPosition.Length){
                        _patrolNum = 0;
                    }
                }

                if(Vector3.Distance(transform.position, _chaseTarget.position) < _detectionRange){
                    _enemyState = State.Chasing;
                }
                break;
            case State.Chasing:
                Move(_chaseTarget.position);
                if(Vector3.Distance(transform.position, _chaseTarget.position) > _chaseRange || Vector3.Distance(transform.position, _originalPosition) > _moveRange){
                    _enemyState = State.Patrolling;
                }

                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    void Attack(){
        //attack
    }

    void Move(Vector3 _aimPosition){
        _onGround = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - 0.3f), new Vector2(0.8f, 1), 0, Vector2.zero, 0, _groundLayer);
        _onWall= Physics2D.BoxCast(transform.position, new Vector2(2, 0.8f), 0, Vector2.zero, 0, _groundLayer);

        //aim at target (x), and move
        _aimDirection = transform.position.x < _aimPosition.x ? 1 : -1;
        _rb.linearVelocity += new Vector2(_moveSpeed * _aimDirection, 0) * Time.deltaTime;

        //climb
        if(transform.position.y < _aimPosition.y + 3 && _onWall){
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _climbSpeed);
        }

        //if the player is a lot higher, jump towards them
        if(_onGround && transform.position.y + 5 < _aimPosition.y){
            Jump();
        }
    }

    void Jump(){
        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
    }
}
