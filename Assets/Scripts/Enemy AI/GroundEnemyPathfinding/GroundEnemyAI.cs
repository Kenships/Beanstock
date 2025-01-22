using NUnit.Framework;
using UnityEngine;

public class GroundEnemyAI : MonoBehaviour
{
     // Enemy Parts
    protected Rigidbody2D rb;
    // IMPLEMENT VISION FOR CHASING USING RAYCASTS
    
    // Interaction with Player
    public PlayerController player;
    public Transform playerTransform;
    public bool hasVision;
    public float speed;    
    public float visionDistance;
    public Transform facingDirection;

    

    // **Enemy Movement** //

    
    //States (can also do states using a string and just write the state, but I guess this is more standard)
    public bool isIdle;
    public bool isPatrolling;
    public bool isChasing;
    
    
    //Patrol
    public Transform[] patrolPoints; //made for two points
    public float patrolPointDistance;
    private int patrolDestination;

    // Chase
    public float chaseSpeed;
    public float chaseDistanceNoVision;

    


    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            Idle();
        }
        else if (isPatrolling)
        {
            Patrol();
        }
        else if (isChasing)
        {
            Chase();
        }
    }

   
    protected void Idle()
    {
        if (hasVision)
        {
            isChasing = true;
            isIdle = false;
        }
    }
    
    protected void Patrol() // need to adjust patrols so that it flips the enemy to the face the correct direction (needs to account for vertical patrols)
    {
        if (patrolDestination == 0)
        {
            // move enemy to point 0
            transform.position = UnityEngine.Vector2.MoveTowards(transform.position, patrolPoints[0].position, speed * Time.deltaTime);
            
            // check when enemy is close to point 0 and change destination to point 1
            if (UnityEngine.Vector2.Distance(transform.position, patrolPoints[0].position) < patrolPointDistance)
            {
                patrolDestination = 1;
            }
        }
        else if (patrolDestination == 1)
        {
            // move enemy to point 1
            transform.position = UnityEngine.Vector2.MoveTowards(transform.position, patrolPoints[1].position, speed * Time.deltaTime);

            // check when enemy is close to point 1 and change destination to point 0
            if (UnityEngine.Vector2.Distance(transform.position, patrolPoints[1].position) < patrolPointDistance)
            {
                patrolDestination = 0;
            }
        }
        
        // Check if player is in chases distance, if so change to chasing state
        // Future: Change this to a raycast/vision check
        if (hasVision)
        {
            isChasing = true;
            isPatrolling = false;
        }

    }

    protected void Chase()
    {
        // When using vision, if the player isn't in the raycast but was being chased and is still in the nearby vicinty the enemy should stay guarding that point/looking around until x time passes
        // When getting jumped over the enemy could either also jump to catch them (might be to hard)
        // or, set a slight delay so that the enemy doesn't immediately turn around and chase the player again, looks more natural that way
        // I tried to do this with invoke but it glitches, setting an if statement to check when the player is right above the enemies head and then pausing (either with Invoke() or WaitSeconds()) for a moment could work

        // Chases player, only along the x-axis
        // If the player is to the left of enemy
        if (transform.position.x > playerTransform.position.x)
        {
            transform.localScale = new UnityEngine.Vector3(1, 1, 1); //flips enemy to face left
            transform.position += UnityEngine.Vector3.left * chaseSpeed * Time.deltaTime; //moves enemy left
        } 
        // If the player is to the right of enemy
        else if (transform.position.x < playerTransform.position.x)
        {   
            transform.localScale = new UnityEngine.Vector3(-1, 1, 1); //flips enemy to face right
            transform.position += UnityEngine.Vector3.right * chaseSpeed * Time.deltaTime;
        }

        // next part will change, view note above
        // Check if the player is out of chase distance, if so change to patrolling state 
        if (!hasVision)
        {
            isChasing = false;
            isIdle = true; //idles by default, can switch to patrolling if needed
        }

    }

    // **Vision** //
   
   
    protected void Vision()
    {
        
        //send raycast to player
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (UnityEngine.Vector3) forwardDirection(), playerTransform.position - transform.position, visionDistance); //!!!!we add forwardDirection to the origin to account for the enemy's collider. Notice if the player is behind the enemy it will hit the enemy collider, is this what we want so that it doesn't target the player behind it, or is it better to use the explicit check that I have set u?
        if (hit.collider != null)
        {
            //check if the raycast hit a player
            if (hit.collider.gameObject.tag == "Player")
            {
                //check if the enemy is facing the player
                if (IsFacingPlayer())
                {
                    Debug.DrawRay(transform.position, playerTransform.position - transform.position, Color.green);
                    hasVision = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, playerTransform.position - transform.position, Color.blue);
                    hasVision = false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, playerTransform.position - transform.position, Color.red);
                Debug.Log(hit.collider.gameObject.tag);
                hasVision = false;
            }
        }
    }

    private bool IsFacingPlayer() //check if enemy is facing the player
    {
        //enemy direction
        UnityEngine.Vector2 enemyDirection = forwardDirection();


        //direction to player
        UnityEngine.Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        //check if enemy is facing the player (true = facing, false = not facing)
        return UnityEngine.Vector2.Dot(enemyDirection, directionToPlayer) > 0;
    }
    
    private UnityEngine.Vector2 forwardDirection()
    {
        if (facingDirection.position.x < transform.position.x )
        {
            return -transform.right;
        }
        else if (facingDirection.position.x > transform.position.x)
        {
            return transform.right;
        }
        // add if horizontal movement, do the same but on the y vertice

        return transform.right; //default to right because no null option for Vector2
    }

    private bool EnterChase()
    {
        if (hasVision || (Vector3.Distance (transform.position, player.transform.position) < chaseDistanceNoVision))
        {
            return true;
        }

        return false;
    }

    private bool ExitChase()
    {
        if (!hasVision )
        {
            return true;
        }
        if ((Vector3.Distance (transform.position, player.transform.position) > chaseDistanceNoVision))
        {
            //start timer
            //once timer is done
        }

        return false;
    }
}
