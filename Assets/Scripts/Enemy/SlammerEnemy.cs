using UnityEngine;
using Util;

public class SlammerEnemy : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Timer _riseTimer;
    private Timer _fallTimer;
    
    private Vector3 originalPosition;
    private const float jumpHeight = 8;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float topWaitTime;
    [SerializeField] private float bottomWaitTime;
    public GameObject attack;
    private bool _rising;

    void Awake()
    {
        originalPosition = transform.position;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _riseTimer = new Timer(topWaitTime);
        _fallTimer = new Timer(bottomWaitTime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _riseTimer.OnTimerEnd += Fall;
        _fallTimer.OnTimerEnd += Rise;
    }

    // Update is called once per frame
    void Update()
    {   
        //rising to top
        if(_rising){
            //rise
            if(transform.position.y < originalPosition.y + jumpHeight){
                _rb.linearVelocity += new Vector2(0, riseSpeed) * Time.deltaTime;
            }
            else{
                //wait at top
                _rb.linearVelocity *= 1 - Time.deltaTime * 20;
                _riseTimer.Tick(Time.deltaTime);
            }
        }
        else{
            //desend
            if(transform.position.y > originalPosition.y + 1){
                _rb.linearVelocity += new Vector2(0, -fallSpeed) * Time.deltaTime;
            }
            else{
                //wait at bottom
                _fallTimer.Tick(Time.deltaTime);
            }
        }
    }

    private void Rise()
    {
        _fallTimer.Restart();
        _rising = true;
    }

    private void Fall()
    {
        _riseTimer.Restart();
        _rising = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ground")){
            Instantiate(attack, transform.position + new Vector3(0, -0.5f), Quaternion.identity);
        }
    }
}
