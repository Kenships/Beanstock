
using System;
using Events.Channels;
using Events.Input;
using Events.Listeners;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float moveSpeed;

    private Vector3 directionalInput;
    private bool onGround;
    private Rigidbody2D rb;
    [SerializeField] private float jumpSpeed;
    private float wallSide;
    [SerializeField] private float climbSpeed; 
    [SerializeField] private Vector2 wallJumpSpeed;
    [SerializeField] private float wallSlideDrag;
    [SerializeField] private Transform playerSprite;
    private float cayoteTime;
    
    private void Awake()
    {
        inputReader.EnablePlayerActions();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start(){
        inputReader.Jump += Jump;
        inputReader.Move += SetDirection;
    }

    private UnityAction<bool> Test()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        //jittery camera
        Move();

        setSprite();
    }

    private void setSprite(){
        //rotate player sprite based on their state

        if(wallSide == 0){
            //in air or on ground
            lerpRotate(playerSprite, -rb.linearVelocityX * 2, 10);
        }
        else if(wallRunInput()){
            //wallrunning
            lerpRotate(playerSprite, wallSide * 90, 15);
        }
        else{
            //sliding down wall
            lerpRotate(playerSprite, wallSide * 10, 10);
        }
    }

    private bool wallRunInput(){
        return directionalInput.x == wallSide && directionalInput.y == 1;
    }

    void lerpRotate(Transform setter, float angle, float speed){
        //rotates the object smoothly to a new angle

        Vector3 originalAngle = setter.eulerAngles;
        setter.eulerAngles = new Vector3(0,0, angle);
        Quaternion to = setter.rotation;

        setter.eulerAngles = originalAngle;
        setter.rotation = Quaternion.SlerpUnclamped(setter.rotation, to, Time.deltaTime * speed);
    }
    
    private void Jump(){
        if(cayoteTime > 0){
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpSpeed);
        }
    }

    private void Move()
    {
        //onground vs. in air movement
        if(turning() && onGround){
            moveHorizontal(2f);
        }
        else{
            moveHorizontal(1);
        }

        setGravity();

        wallInteraction();

        cayoteTime -= Time.deltaTime;
        if(onGround == true){
            cayoteTime = 0.15f; 
        }
    }

    private void wallInteraction(){
        //if the players touching a wall
        if(wallSide != 0 && onGround == false){
            if(directionalInput.y == 1){
                if(directionalInput.x == wallSide){
                    //wall running
                    rb.linearVelocity = new Vector2(rb.linearVelocityX, climbSpeed);
                }
                else if(directionalInput.x == -wallSide){
                    //wall jump
                    rb.linearVelocity = new Vector2(directionalInput.x * wallJumpSpeed.x, wallJumpSpeed.y);
                }
            }
            else if(directionalInput.x == wallSide && rb.linearVelocityY < 0){
                //sliding down wall
                rb.linearVelocity *= new Vector2(1, 1 - Time.deltaTime * wallSlideDrag);
            }
        }
    }

    private void setGravity(){
        if(rb.linearVelocityY > 2){
            rb.gravityScale = 3;
        }
        else{
            rb.gravityScale = 14;
        }
    }

    private void moveHorizontal(float speedFactor){
        rb.linearVelocity += new Vector2(directionalInput.x * moveSpeed, 0) * speedFactor * Time.deltaTime;
    } 

    private bool turning(){
        float direction = rb.linearVelocityX / Mathf.Abs(rb.linearVelocityX);
        return direction != directionalInput.x;
    }

    public void SetDirection(Vector2 dir)
    {
        directionalInput = dir;
    }

    public void SetOnGround(bool set){
        onGround = set;
    }

    public void SetOnWall(int set){
        wallSide = set;
    }
}
