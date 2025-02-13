using System.Collections;
using System.Collections.Generic;
using DamageManagement;
using Events.Channels;
using Events.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

public class PlayerController : MonoBehaviour
{
    [Header("___EVENTS___")]
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private InputReader inputReader;
    
    [Header("___MOVEMENT___")]
    [SerializeField] private Vector2 wallJumpSpeed;
    [SerializeField] private GameObject attack;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float wallSlideDrag;
    
    [Header("___DASH ATTACK___")]
    [SerializeField] private float playerDamage;
    [SerializeField] private float dashCompleteSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackLength;
    [SerializeField] private float attackRange;
    
    [Header("___PARTICLES___")]
    [SerializeField] private GameObject sliceEffect;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem wallJumpEffect;
    [SerializeField] private ParticleSystem wallRunEffect;
    
    [Header("___TIMER CONFIG___")]
    [SerializeField] private float invincibilityMax;
    [SerializeField] private float cayoteTimeMax;
    [SerializeField] private float wallCayoteTimeMax;
    [SerializeField] private float DashAttackCooldown;
    [SerializeField] private float DashBufferMax;
    
    [Header("___AXE CONFIG___")]
    [SerializeField] private Transform axe;
    [SerializeField] private SpriteRenderer axeSprite;
    [SerializeField] private float axeSpinningSpeed;
    
    [Header("___RADAR DEBUG___")]
    [SerializeField] List<GameObject> enemiesInRadar;
    

    private const float AxeFollowSpeed = 50;
    private const float AxeRotationSpeed = 300;
    private const float AttackSlowDown = 1.3f;
    private const float TurningSpeed = 2f;
    private const float RunningSpeed = 1f;
    private const float FallGravity = 14f;
    private const float RiseGravity = 3f;
    private const float WallRunLingerTime = 0.01f;
    
    private State _playerState;
    private ParticleSystem _attackEffect;
    private Vector3 _directionalInput;
    private Rigidbody2D _rb;
    private List<Timer> _timers;
    private Timer _wallRunTimer;
    private Timer _groundCayoteTime;
    private Timer _wallCayoteTime;
    private Timer _invincibility;
    private Timer _attackTimer;
    private Timer _dashAttackCooldown;
    private Timer _dashAttackBuffer;
    private bool _onGround;
    private bool _canAttack;
    private float _wallSide;
    
    private float _direction;

    private void Awake()
    {
        sliceEffect.SetActive(false);
        _attackEffect = attack.GetComponent<ParticleSystem>();
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        
        /*___TIMER INITIALIZATION___*/
        //ticks separately
        _groundCayoteTime = new Timer(cayoteTimeMax);
        
        
        _timers = new List<Timer>();
        //Initialize timers and add them to the list --> UpdateTimers() will update all timers in the list
        _wallRunTimer = new Timer(WallRunLingerTime);
        _timers.Add(_wallRunTimer);
        _wallCayoteTime = new Timer(wallCayoteTimeMax);
        _timers.Add(_wallCayoteTime);
        _invincibility = new Timer(invincibilityMax);
        _timers.Add(_invincibility);
        _attackTimer = new Timer(attackLength);
        _timers.Add(_attackTimer);
        _dashAttackCooldown = new Timer(DashAttackCooldown);
        _timers.Add(_dashAttackCooldown);
        _dashAttackBuffer = new Timer(0);
        _timers.Add(_dashAttackBuffer);
        
        enemiesInRadar = new List<GameObject>();
        
    }

    private void Start(){
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
        inputReader.Attack.onEventRaised += AttackStart;
        _wallRunTimer.OnTimerStart += StartWallRunningParticles;
        _wallRunTimer.OnTimerEnd += StopWallRunningParticles;
        _wallCayoteTime.OnTimerEnd += DetachFromWall;
        _attackTimer.OnTimerEnd += EndDashAttack;
        _dashAttackCooldown.OnTimerStart += () => _canAttack = false;
        _dashAttackCooldown.OnTimerEnd += () => _canAttack = true;
        _dashAttackBuffer.OnTimerEnd += ForceStartDashAttack;
        SetState(State.Moving);
    }
    
    private enum State{
        Moving,
        Attacking,
        Ziplining
    }
    
    private void Update()
    {   
        switch(_playerState){
            case State.Moving:
                gameObject.tag = "Player";
                SetRunningSpriteRotation();
                Move();
                break;
            case State.Attacking:
                gameObject.tag = "Player Attack";
                DashMovement();
                break;
            case State.Ziplining:
                break;
        }
        //jittery camera
        SetGravity();
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        float deltaTime = Time.deltaTime;
        foreach (Timer timer in _timers)
        {
            timer.Tick(deltaTime);
        }
    }

    private void FixedUpdate() {
        SetAxe();
    }

    private void SetRunningSpriteRotation()
    {
        if(_wallSide == 0){
            //in air or on ground
            if(_onGround){
                SlerpRotate(playerSprite, -_rb.linearVelocityX * 2, 10);
            }
            else{
                SlerpRotate(playerSprite, -_rb.linearVelocityX * -1.5f, 10);
            }
        }
        else if(WallRunInput()){
            //wallrunning
            SlerpRotate(playerSprite, _wallSide * 90, 15);
            _wallRunTimer.Restart(WallRunLingerTime);
        }
        else{
            //sliding down wall
            SlerpRotate(playerSprite, _wallSide * 10, 10);
        }
    }

    private void SetAxe(){
        axe.position = Vector3.Lerp(axe.position, transform.position + new Vector3(_directionalInput.x * 0.2f, 0), AxeFollowSpeed * Time.deltaTime);

        if(_playerState == State.Moving){
            SlerpRotate(axe, _directionalInput.x * 30, AxeRotationSpeed * Time.deltaTime);
        }
        else if(_playerState == State.Attacking){
            //SlerpRotate(axe, direction * -170, _axeRotationSpeed * Time.deltaTime);
            axe.Rotate(0, 0, -axeSpinningSpeed * _direction * Time.deltaTime);
        }

        axeSprite.flipX =  (int) _direction != 1;
    }

    private void StartWallRunningParticles()
    {
        if(wallRunEffect.isPlaying) return;
        wallRunEffect.Play();
    }

    private void StopWallRunningParticles()
    {
        if(!wallRunEffect.isPlaying) return;
        wallRunEffect.Stop();
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
        
        if(_groundCayoteTime.IsRunning){
            Debug.Log("Jump");
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpSpeed);
            _groundCayoteTime.ForceEnd();
        }
    }

    

    private Vector3 GetAimPosition(Vector3 a, Vector3 b){
        return new Vector3(a.x - b.x, a.y - b.y) * -1;
    }

    private void Move()
    {
        //onground vs. in air movement
        if(Turning() && _onGround){
            MoveHorizontal(TurningSpeed);
        }
        else{
            MoveHorizontal(RunningSpeed);
        }

        WallInteraction();
        if(!_onGround)
            _groundCayoteTime.Tick(Time.deltaTime);
    }

    private void WallInteraction(){
        //if the players touching a wall
        
        
            
        

        if (_wallSide != 0)
        {
            if(WallRunInput()){
                //wall running
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
                _wallRunTimer.Restart(WallRunLingerTime);
            }
            else if(Mathf.Approximately(_directionalInput.x, -_wallSide) && Mathf.Approximately(_directionalInput.y, 1)){
                //wall jump
                wallJumpEffect.Play();
                _wallCayoteTime.ForceEnd();
                _rb.linearVelocity = new Vector2(_directionalInput.x * wallJumpSpeed.x, wallJumpSpeed.y);
            }
            else if (Mathf.Approximately(_directionalInput.x, _wallSide) && _rb.linearVelocityY < 0)
            {
                //sliding down wall
                _rb.linearVelocity *= new Vector2(1, 1 - Time.deltaTime * wallSlideDrag);
            }
        }
            
    }

    private void SetGravity()
    {
        switch(_playerState){
            case State.Moving:
                _rb.gravityScale = _rb.linearVelocityY > 2 ? RiseGravity : FallGravity;
                break;
            case State.Attacking:
                _rb.gravityScale = 0;
                break;
            case State.Ziplining:
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
        Debug.Log("OnGround = " + set);
        _onGround = set;
        if (_onGround)
        {
            _groundCayoteTime.Restart(cayoteTimeMax);
        }
    }

    public void SetOnWall(int set){
        if (set == 0)
        {
            _wallCayoteTime.Restart(wallCayoteTimeMax);
            return;
        }
        _wallCayoteTime.Cancel();
        _wallSide = set;
    }

    public void DetachFromWall()
    {
        _wallSide = 0;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        GetHit(other.tag);

        if(other.CompareTag("Zipline")){
            _playerState = State.Ziplining;
            other.GetComponent<Zipline>().StartZip();
        }
    }

    private void OnParticleCollision(GameObject other) {
        GetHit(other.tag);
    }

    private void GetHit(string otherTag){
        if(!_invincibility.IsRunning && otherTag == "Enemy Attack"){
            hitEffect.Play();
            //TimeController.setTime(0.05f);
            StartCoroutine(TimeController.FreezeTime(0.01f));
            _invincibility.Restart(invincibilityMax);
        }
    }
    private void ForceStartDashAttack(){
        //insures that the player can attack after the buffer ends
        _dashAttackCooldown.ForceEnd();
        AttackStart(new EmptyEventArgs());
    }
    private void AttackStart(EmptyEventArgs args){
        
        if(_playerState == State.Moving && _canAttack){
            if (TryLocateClosestTarget(out GameObject closestTarget))
            {
                onAttackEnable.RaiseEvent(true);
                Vector3 target = closestTarget.transform.position;

                if(Vector3.Distance(transform.position, target) <= attackRange){
                    //start attack
                    sliceEffect.SetActive(true);
                    SetState(State.Attacking);
                    _attackTimer.Restart(attackLength);
                    playerSprite.up = GetAimPosition(transform.position, target);
                }
            }
        }
        else if (!_canAttack && !_dashAttackBuffer.IsRunning && _dashAttackCooldown.RemainingSeconds < DashBufferMax)
        {
            _dashAttackBuffer.Restart(_dashAttackCooldown.RemainingSeconds);
        }
    }
    private void DashMovement()
    {
        _rb.linearVelocity = playerSprite.up * attackSpeed;
    }

    
    public void OnAttackLanded(IDamageable damageable)
    {
        damageable.Damage(playerDamage);

        attack.transform.up = playerSprite.up;
        
        _attackEffect.Play();
        
        //ends dash
        _attackTimer.ForceEnd();
        
        StartCoroutine(DashFollowThrough(0.006f));
    }
    private void EndDashAttack()
    {
         _rb.linearVelocity /= AttackSlowDown;
         SetState(State.Moving);
         sliceEffect.SetActive(false);
         _dashAttackCooldown.Restart(DashAttackCooldown);
         onAttackEnable.RaiseEvent(false);
    }
    
    IEnumerator DashFollowThrough(float time)
    {
        yield return TimeController.FreezeTime(time);;
        _rb.linearVelocity = playerSprite.up * dashCompleteSpeed;
    }
    
    public void ProcessBogie(RadarInfo radarInfo)
    {
        GameObject potentialEnemy = radarInfo.Bogie;

        if (!potentialEnemy.CompareTag("Enemy")) return;
        
        if (radarInfo.InRange)
        {
            AddEnemyToRadar(potentialEnemy);
        }
        else
        {
            RemoveEnemyFromRadar(potentialEnemy);
        }
    }
    private void AddEnemyToRadar(GameObject enemy)
    {
        enemiesInRadar.Add(enemy);
    }

    private void RemoveEnemyFromRadar(GameObject enemy)
    {
        enemiesInRadar.Remove(enemy);
    }

    private bool TryLocateClosestTarget(out GameObject closestTarget)
    {
        closestTarget = null;
        if (enemiesInRadar.Count == 0) return false;
        
        Vector3 myPos = transform.position;
        closestTarget = enemiesInRadar[0];
        float closestDistance = Vector3.Distance(closestTarget.transform.position, myPos);
        foreach (GameObject enemy in enemiesInRadar)
        {
            float distance = Vector3.Distance(enemy.transform.position, myPos);
            if (distance < closestDistance)
            {
                closestTarget = enemy;
                closestDistance = distance;
            }
        }
        
        return true;
    }

    public void EndZipline(Vector3 inputVelocity){
        _rb.linearVelocity = new Vector2(inputVelocity.x, 10);
        //_rb.linearVelocity = inputVelocity * ziplineLaunchSpeed;
        _playerState = State.Moving;
    }

    private void SetState(State newState)
    {
        _playerState = newState;
    }
}
