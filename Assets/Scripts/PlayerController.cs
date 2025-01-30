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
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float playerDamage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private Vector2 wallJumpSpeed;
    [SerializeField] private float wallSlideDrag;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private float cayoteTimeMax;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackLength;
    [SerializeField] private float maxSpinTime;
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject attack;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem wallJumpEffect;
    [SerializeField] private GameObject sliceEffect;
    [SerializeField] private ParticleSystem wallRunEffect;
    [SerializeField] private float invincibilityMax;
    [SerializeField] private Transform axe;
    [SerializeField] private SpriteRenderer axeSprite;
    [SerializeField] private float axeSpinningSpeed;
    [SerializeField] private Vector2 spinHitVelocity;
    [SerializeField] private float spinSpeed;
    [SerializeField] List<GameObject> enemiesInRadar;
    

    private const float AxeFollowSpeed = 50;
    private const float AxeRotationSpeed = 300;
    private const float AttackSlowDown = 1.3f;
    private const float TurningSpeed = 2f;
    private const float RunningSpeed = 1f;
    private const float FallGravity = 14f;
    private const float RiseGravity = 3f;
    private const float WallRunLingerTime = 0.03f;
    
    private State _playerState;
    private ParticleSystem _attackEffect;
    private Vector3 _directionalInput;
    private Rigidbody2D _rb;
    private Timer _wallRunTimer;
    private Timer _cayoteTime;
    private Timer _invincibility;
    private Timer _attackCounter;
    private Timer _spinTimer;
    private bool _onGround;
    private float _wallSide;
    
    private float _direction;

    private void Awake()
    {
        sliceEffect.SetActive(false);
        _attackEffect = attack.GetComponent<ParticleSystem>();
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _cayoteTime = new Timer(cayoteTimeMax);
        _wallRunTimer = new Timer(WallRunLingerTime);
        _invincibility = new Timer(invincibilityMax);
        _attackCounter = new Timer(attackLength);
        _spinTimer = new Timer(maxSpinTime);
        enemiesInRadar = new List<GameObject>();
        SetState(State.Moving);
    }

    private void Start(){
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
        inputReader.Attack.onEventRaised += AttackAction;
        _wallRunTimer.OnTimerStart += StartWallRunningParticles;
        _wallRunTimer.OnTimerEnd += StopWallRunningParticles;
        _attackCounter.OnTimerEnd += EndDashAttack;
        _spinTimer.OnTimerEnd += StopSpinning;
    }

    private enum State{
        Moving,
        Attacking,
        Ziplining,
        Spinning
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
                DashAttack();
                break;
            case State.Ziplining:
                break;
            case State.Spinning:
                gameObject.tag = "Player Attack";
                SetSpinningSpriteRotation();
                Move();
                break;
        }
        //jittery camera
        SetGravity();
        _spinTimer.Tick(Time.deltaTime);
        _invincibility.Tick(Time.deltaTime);
        _wallRunTimer.Tick(Time.deltaTime);
        _attackCounter.Tick(Time.deltaTime);
    }

    private void FixedUpdate() {
        SetAxe();
    }

    private void SetSpinningSpriteRotation()
    {
        playerSprite.Rotate(0, 0, spinSpeed * Time.deltaTime * _direction);
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
        else if(_playerState == State.Spinning){
            axe.Rotate(0, 0, spinSpeed * _direction * Time.deltaTime);
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

    private void DashAttack()
    {
        _rb.linearVelocity = playerSprite.up * attackSpeed;

        if(!_attackCounter.IsRunning){
            _rb.linearVelocity /= AttackSlowDown;
            SetState(State.Moving);
        }
    }

    private void EndDashAttack()
    {
        _rb.linearVelocity /= AttackSlowDown;
        sliceEffect.SetActive(false);
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
            _cayoteTime.ForceEnd();
        }
    }

    private void AttackAction(EmptyEventArgs args){
        
        if(_playerState == State.Moving){
            if (TryLocateClosestTarget(out GameObject closestTarget))
            {
                onAttackEnable.RaiseEvent(true);
                Vector3 target = closestTarget.transform.position;

                if(Vector3.Distance(transform.position, target) <= attackRange){
                    //start attack
                    sliceEffect.SetActive(true);
                    SetState(State.Attacking);
                    _attackCounter.Restart(attackLength);
                    playerSprite.up = GetAimPosition(transform.position, target);
                }
            }
            
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
            _cayoteTime.Tick(Time.deltaTime);

        if(_directionalInput.x != 0 && _playerState != State.Spinning)
            _direction = _directionalInput.x;
    }

    private void WallInteraction(){
        //if the players touching a wall
        if(_wallSide != 0 && !_onGround){
            if(WallRunInput()){
                //wall running
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
            }
            else if(Mathf.Approximately(_directionalInput.x, -_wallSide) && Mathf.Approximately(_directionalInput.y, 1)){
                //wall jump
                wallJumpEffect.Play();
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
                _rb.gravityScale = _rb.linearVelocityY > 2 ? RiseGravity : FallGravity;
                break;
            case State.Attacking:
                _rb.gravityScale = 0;
                break;
            case State.Ziplining:
                _rb.gravityScale = 0;
                break;
            case State.Spinning:
                _rb.gravityScale = _rb.linearVelocityY > 2 ? RiseGravity : FallGravity + 5;
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
            if (_playerState == State.Spinning)
            {
                _attackCounter.ForceEnd();
                SetState(State.Moving);
                
            }
                
        }
    }

    public void SetOnWall(int set){
        _wallSide = set;

        if (_playerState == State.Spinning && set != 0)
        {
            _attackCounter.ForceEnd();
            SetState(State.Moving);
        }
            
    }

    private void StopSpinning()
    {
        //don't use set state to avoid double invokation
        _playerState = State.Moving;
        onAttackEnable.RaiseEvent(false);
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

    public void CompleteAttack(IDamageable damageable)
    {
        damageable.Damage(playerDamage);
        
        _rb.linearVelocity = new Vector3(spinHitVelocity.x * _directionalInput.x, spinHitVelocity.y);
        _attackEffect.Play();

        StartCoroutine(TimeController.FreezeTime(0.006f));
        _spinTimer.Restart(maxSpinTime);
        if(_playerState == State.Spinning){
            
            if(_directionalInput.x == 0){
                attack.transform.up = Vector3.down;
            }
            else{
                attack.transform.eulerAngles = new Vector3(0, 0, _directionalInput.x * -210);
            }
        }
        else{
            attack.transform.up = playerSprite.up;
            _playerState = State.Spinning;
        }
        
        
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
        //stop spinning on transition out from spinning state
        if (_playerState == State.Spinning && _playerState != newState)
        {
            _spinTimer.ForceEnd();
        }
        _playerState = newState;
    }
}
