
using Events.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float moveSpeed;
    private Vector3 directionalInput;
    private bool onGround;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        inputReader.EnablePlayerActions();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        /*
        if (moveDir.sqrMagnitude > 0.01f)
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }
        */

        rb.linearVelocity += new Vector2(directionalInput.x * moveSpeed, 0);

        if(turning()){
            moveHorizontal(1.5f);
        }
        else{
            moveHorizontal(1);
        }
    }

    private void moveHorizontal(float speedFactor){
        rb.linearVelocity += new Vector2(directionalInput.x * moveSpeed, 0) * speedFactor * Time.deltaTime;
    } 

    private bool turning(){
        float direction = rb.linearVelocityX / Mathf.Abs(rb.linearVelocityX);
        return direction == directionalInput.x;
    }

    public void SetDirection(Vector2 dir)
    {
        directionalInput = dir;
    }

    public void SetOnGround(bool set){
        onGround = set;
    }
}
