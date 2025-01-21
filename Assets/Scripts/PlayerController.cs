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
    private Vector3 _directionalInput;
    private bool _onGround;
    private Rigidbody2D _rb;
    private float _wallSide;
    private Timer _cayoteTime;
    
    private void Awake()
    {
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _cayoteTime = new Timer(cayoteTimeMax);
    }

    private void Start(){
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
    }
    
    private void Update()
    {
        //jittery camera
        Move();

        SetSprite();
    }

    private void SetSprite(){
        //rotate player sprite based on their state

        if(_wallSide == 0){
            //in air or on ground
            SlerpRotate(playerSprite, -_rb.linearVelocityX * 2, 10);
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

    private bool WallRunInput(){
        return Mathf.Approximately(_directionalInput.x, _wallSide) && Mathf.Approximately(_directionalInput.y, 1);
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

    private void Move()
    {
        //onground vs. in air movement
        if(Turning() && _onGround){
            MoveHorizontal(2f);
        }
        else{
            MoveHorizontal(1);
        }

        SetGravity();

        WallInteraction();
        if(!_onGround)
            _cayoteTime.Tick(Time.deltaTime);
    }

    private void WallInteraction(){
        //if the players touching a wall
        if(_wallSide != 0 && _onGround == false){
            if(Mathf.Approximately(_directionalInput.y, 1)){
                if(Mathf.Approximately(_directionalInput.x, _wallSide)){
                    //wall running
                    _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
                }
                else if(Mathf.Approximately(_directionalInput.x, -_wallSide)){
                    //wall jump
                    _rb.linearVelocity = new Vector2(_directionalInput.x * wallJumpSpeed.x, wallJumpSpeed.y);
                }
            }
            else if(Mathf.Approximately(_directionalInput.x, _wallSide) && _rb.linearVelocityY < 0){
                //sliding down wall
                _rb.linearVelocity *= new Vector2(1, 1 - Time.deltaTime * wallSlideDrag);
            }
        }
    }

    private void SetGravity()
    {
        _rb.gravityScale = _rb.linearVelocityY > 2 ? 3 : 14;
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
}
