using Events.Channels;
using Events.Input;
using UnityEngine;
using Util;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private Vector2 wallJumpSpeed;
    [SerializeField] private float wallSlideDrag;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private float cayoteTimeMax;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackLength;
    [SerializeField] private float attackRange;
    private Vector3 _directionalInput;
    private bool _onGround;
    private Rigidbody2D _rb;
    private float _wallSide;
    private Timer _cayoteTime;
    private State _playerState;
    private float attackCounter;
    private const float attackSlowDown = 1.5f;
    private const float turningSpeed = 2;
    private const float runningSpeed = 1;
    private const float fallGravity = 14;
    private const float riseGravity = 3;
    
    private void Awake()
    {
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _cayoteTime = new Timer(cayoteTimeMax);
        _playerState = State.Moving;
    }

    private void Start(){
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
        inputReader.Attack.onEventRaised += AttackAction;
    }

    private enum State{
        Moving,
        Attacking
    }
    
    private void Update()
    {   
        switch(_playerState){
            case State.Moving:
                gameObject.tag = "Player";
                Move();
                break;
            case State.Attacking:
                gameObject.tag = "Player Attack";
                Attack();
                break;
        }
        //jittery camera

        SetSprite();
        SetGravity();
    }

    private void SetSprite(){
        //rotate player sprite based on their state

        if(_playerState == State.Moving){
            if(_wallSide == 0){
                //in air or on ground
                if(_onGround == true){
                    SlerpRotate(playerSprite, -_rb.linearVelocityX * 2, 10);
                }
                else{
                    SlerpRotate(playerSprite, -_rb.linearVelocityX * -1.5f, 10);
                }
            }
            else if(WallRunInput()){
                //wallrunning
                SlerpRotate(playerSprite, _wallSide * 90, 15);
            }
            else{
                //sliding down wall
                SlerpRotate(playerSprite, _wallSide * 10, 10);
            }
        }
    }

    private bool WallRunInput(){
        return Mathf.Approximately(_directionalInput.x, _wallSide) && Mathf.Approximately(_directionalInput.y, 1);
    }

    void Attack(){
        _rb.linearVelocity = playerSprite.up * attackSpeed;
        attackCounter -= Time.deltaTime;


        if(attackCounter < 0){
            _rb.linearVelocity /= attackSlowDown;
            _playerState = State.Moving;
        }
    }

    private void SlerpRotate(Transform setter, float angle, float speed){
        //rotates the object smoothly to a new angle

        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }
    
    private void Jump(EmptyEventArgs args){
        if(_cayoteTime.IsRunning){
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpSpeed);
        }
    }

    private void AttackAction(EmptyEventArgs args){
        if(_playerState == State.Moving){
            Vector3 target = findClosest("Enemy");

            if(Vector3.Distance(transform.position, target) <= attackRange && target != Vector3.zero){
                _playerState = State.Attacking;
                attackCounter = attackLength;
                playerSprite.up = AimAt(transform.position, target);
            }
        }
    }

    private Vector3 findClosest(string tag){
        //find all enemies
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        //make sure there is enemies
        if(targets == null){
            return Vector3.zero;
        }
        else{
            float closest = Vector3.Distance(transform.position, targets[0].transform.position);
            GameObject closestTarget = targets[0];

            //find closest
            foreach(GameObject check in targets){
                float dist = Vector3.Distance(transform.position, check.transform.position);
                if(dist < closest){
                    closest = dist;
                    closestTarget = check;
                }
            }

            return closestTarget.transform.position;
        }
    }

    private Vector3 AimAt(Vector3 a, Vector3 b){
        return new Vector3(a.x - b.x, a.y - b.y) * -1;
    }

    private void Move()
    {
        //onground vs. in air movement
        if(Turning() && _onGround){
            MoveHorizontal(turningSpeed);
        }
        else{
            MoveHorizontal(runningSpeed);
        }

        WallInteraction();
        if(!_onGround)
            _cayoteTime.Tick(Time.deltaTime);
    }

    private void WallInteraction(){
        //if the players touching a wall
        if(_wallSide != 0 && _onGround == false){
            if(WallRunInput()){
                //wall running
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
            }
            else if(Mathf.Approximately(_directionalInput.x, -_wallSide) && Mathf.Approximately(_directionalInput.y, 1)){
                //wall jump
                _rb.linearVelocity = new Vector2(_directionalInput.x * wallJumpSpeed.x, wallJumpSpeed.y);
            }
            else if(Mathf.Approximately(_directionalInput.x, _wallSide) && _rb.linearVelocityY < 0){
                //sliding down wall
                _rb.linearVelocity *= new Vector2(1, 1 - Time.deltaTime * wallSlideDrag);
            }
        }
    }

    private void SetGravity()
    {
        switch(_playerState){
            case State.Moving:
                _rb.gravityScale = _rb.linearVelocityY > 2 ? riseGravity : fallGravity;
                break;
            case State.Attacking:
                _rb.gravityScale = 0;
                break;
        }
    }

    private void MoveHorizontal(float speedFactor){
        _rb.linearVelocity += new Vector2(_directionalInput.x * moveSpeed, 0) * (speedFactor * Time.deltaTime);
    } 

    private bool Turning(){
        float direction = _rb.linearVelocityX / Mathf.Abs(_rb.linearVelocityX);
        return !Mathf.Approximately(direction, _directionalInput.x);
    }
    public void SetDirection(Vector2 dir)
    {
        _directionalInput = dir;
    }

    public void SetOnGround(bool set){
        _onGround = set;
        if (_onGround)
        {
            _cayoteTime.Restart(cayoteTimeMax); 
        }
    }

    public void SetOnWall(int set){
        _wallSide = set;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Enemy" && _playerState == State.Attacking){
            attackCounter = 0.1f;
        }
    }
}
