using UnityEngine;

public class SlammerEnemy : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float attackCounter;
    private Vector3 originalPosition;
    private const float jumpHeight = 5;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float topWaitTime;
    [SerializeField] private float bottomWaitTime;
    public GameObject attack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {   
        //rising to top
        if(attackCounter < topWaitTime){
            //rise
            if(transform.position.y < originalPosition.y + jumpHeight){
                _rb.linearVelocity += new Vector2(0, riseSpeed) * Time.deltaTime;
            }
            else{
                //wait at top
                _rb.linearVelocity *= 1 - Time.deltaTime * 20;
                attackCounter += Time.deltaTime;
            }
        }
        else{
            //desend
            if(transform.position.y > originalPosition.y + 1){
                _rb.linearVelocity += new Vector2(0, -fallSpeed) * Time.deltaTime;
            }
            else{
                //wait at bottom
                attackCounter += Time.deltaTime;

                if(attackCounter > bottomWaitTime + topWaitTime){
                    attackCounter = 0;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Ground"){
            Instantiate(attack, transform.position + new Vector3(0, -0.5f), Quaternion.identity);
        }
    }
}
