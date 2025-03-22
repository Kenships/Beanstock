using DamageManagement;
using Enemy;
using Events.Channels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Timer = Util.Timer;

public class SlammerEnemy : AbstractEnemy
{
    private Timer _riseTimer;
    private Timer _fallTimer;
    private const float JumpHeight = 8;
    [Header("___Slammer Config___")]
    [SerializeField] private float riseSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float topWaitTime;
    [SerializeField] private float bottomWaitTime;
    [SerializeField] private AttackCollider attackCollider;
    public GameObject attack;
    private bool _rising;
    [SerializeField] private Animation jumpAnimation;
    private AudioManager _audioManager;

    private new void Awake()
    {
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        base.Awake();
        _riseTimer = new Timer(topWaitTime);
        _fallTimer = new Timer(bottomWaitTime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private new void Start()
    {
        base.Start();
        _riseTimer.OnTimerEnd += Fall;
        _fallTimer.OnTimerEnd += Rise;
        AttackDurationTimer.OnTimerStart += StartAttack;
        AttackDurationTimer.OnTimerEnd += EndAttack;
        attackCollider.OnAttackGameObject.onEventRaised += AttackObject;
        groundCheckCollider.GroundEvent.onEventRaised += SetGrounded;

        Transform attackTransform = attackCollider.gameObject.transform;
        attackCollider.gameObject.transform.localScale = new Vector3(attackRange * attackTransform.localScale.x, attackTransform.localScale.y);
    }

    private void AttackObject(GameObject bogie)
    {
        if (bogie.CompareTag("Player"))
        {
            bogie.GetComponent<IDamageable>().Damage(attackDamage);
        }
    }
    
   
    // Update is called once per frame
    void Update()
    {   
        //rising to top
        if(_rising){
            if(!AttackDurationTimer.IsRunning){
                AttackDurationTimer.Restart(attackDuration); 
                jumpAnimation.Play();
                _audioManager.PlaySFX(_audioManager.whoosh, transform.position);
            }

            //rise
            if(transform.position.y < _originalPosition.y + JumpHeight){
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
            if(transform.position.y > _originalPosition.y + 1){
                _rb.linearVelocity += new Vector2(0, -fallSpeed) * Time.deltaTime;
            }
            else{
                //wait at bottom
                AttackDurationTimer.Tick(Time.deltaTime);
                _fallTimer.Tick(Time.deltaTime);
            }
        }
    }

    private void StartAttack()
    {
        attackCollider.OnAttackEnable.RaiseEvent(true);
    }

    private void EndAttack()
    {
        attackCollider.OnAttackEnable.RaiseEvent(false);
        //_audioManager.PlaySFX(_audioManager.bossSmash);
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
    private void SetGrounded(bool grounded)
    {
        
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ground")){
            _audioManager.PlaySFX(_audioManager.bossSmash, transform.position);
            Instantiate(attack, transform.position + new Vector3(0, -0.5f), Quaternion.identity);
        }
    }
}
