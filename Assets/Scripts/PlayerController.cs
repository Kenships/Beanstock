using System;
using System.Collections;
using System.Collections.Generic;
using DamageManagement;
using DefaultNamespace;
using Enemy;
using Events.Channels;
using Events.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Util;
[RequireComponent(typeof(HealthManager))]
public class PlayerController : MonoBehaviour, ICanZipline
{
    [Header("___EVENTS___")]
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private InputReader inputReader;
    [Header("___HEALTH___")]
    [SerializeField] private HealthManager healthManager;
    [Header("___MOVEMENT___")]
    [SerializeField] private Vector2 wallJumpSpeed;
    [SerializeField] private GameObject attack;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float wallSlideDrag;
    [SerializeField] private float ziplineBoost;
    
    [Header("___DASH ATTACK___")]
    [SerializeField] private float playerDamage;
    [SerializeField] private float dashCompleteSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackLength;
    [SerializeField] private float attackRange;
    
    [Header("___PARTICLES___")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem wallJumpEffect;
    [SerializeField] private ParticleSystem sliceEffect;
    [SerializeField] private ParticleSystem wallRunEffect;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem ziplineEffect;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private Animation transition;
    [SerializeField] private ParticleSystem checkPointEffect;
    
    [Header("___TIMER CONFIG___")]
    [SerializeField] private float invincibilityMax;
    [SerializeField] private float cayoteTimeMax;
    [SerializeField] private float wallCayoteTimeMax;
    [SerializeField] private float dashAttackCooldown; 
    [SerializeField] private float dashBufferMax;
    
    [Header("___AXE CONFIG___")]
    [SerializeField] private Transform axe;
    [SerializeField] private SpriteRenderer axeSprite;
    [SerializeField] private float axeSpinningSpeed;
    
    [Header("___SHOTGUN CONFIG___")]
    [SerializeField] private float shotSpeed;
    [SerializeField] private float reloadTime;
    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private Transform shotTransform;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("___RADAR DEBUG___")]
    [SerializeField] List<GameObject> enemiesInRadar;

    [SerializeField] private GameObject currentTarget;
    
    
    [Header("___ANIMATION___")]
    [FormerlySerializedAs("_spriteRenderer")][SerializeField] private SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    
    [Header("___CHECKPOINT___")]
    [SerializeField] private CheckPoint defaultCheckPoint;
    [SerializeField] private GameObject respawnHolder;
    public static float killHeight;

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
    [SerializeField] private CheckPoint _checkPoint;
    private Timer _wallRunTimer;
    private Timer _groundCayoteTime;
    private Timer _wallCayoteTime;
    private Timer _invincibility;
    private Timer _attackTimer;
    private Timer _dashAttackCooldown;
    private Timer _dashAttackBuffer;
    private Timer _attackReloadTimer;
    private bool _onGround;
    private bool _canAttack;
    private float _wallSide;
    private float _direction;
    private AudioManager _audioManager;
    
    /* Walking sound field */
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    
    private void Awake()
    {
        
        _attackEffect = attack.GetComponent<ParticleSystem>();
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        
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
        _dashAttackCooldown = new Timer(dashAttackCooldown);
        _timers.Add(_dashAttackCooldown);
        _dashAttackBuffer = new Timer(0);
        _timers.Add(_dashAttackBuffer);
        _attackReloadTimer = new Timer(reloadTime);
        
        enemiesInRadar = new List<GameObject>();
    }

    private void Start()
    {
        healthManager.Reset();
    }

    private void OnEnable(){
        healthManager.Initialize();
        healthManager.OnDamage.onEventRaised += OnDamaged;
        healthManager.OnHeal.onEventRaised += OnHeal;
        healthManager.OnDie.onEventRaised += OnDie;
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
        inputReader.Move.onEventRaised += SelectEnemy;
        inputReader.Attack.onEventRaised += AttackStart;
        inputReader.Shoot.onEventRaised += Shoot;
        _wallRunTimer.OnTimerStart += StartWallRunningParticles;
        _wallRunTimer.OnTimerEnd += StopWallRunningParticles;
        _wallCayoteTime.OnTimerEnd += DetachFromWall;
        _attackTimer.OnTimerEnd += EndDashAttack;
        _dashAttackCooldown.OnTimerStart += DisableAttack;
        _dashAttackCooldown.OnTimerEnd += EnableAttack;
        _dashAttackBuffer.OnTimerEnd += ForceStartDashAttack;
        SetState(State.Moving);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        TimeController.SetTime(1);
        
        healthManager.OnDamage.onEventRaised -= OnDamaged;
        healthManager.OnHeal.onEventRaised -= OnHeal;
        healthManager.OnDie.onEventRaised -= OnDie;
        inputReader.Jump.onEventRaised -= Jump;
        inputReader.Move.onEventRaised -= SetDirection;
        inputReader.Move.onEventRaised -= SelectEnemy;
        inputReader.Attack.onEventRaised -= AttackStart;
        inputReader.Shoot.onEventRaised -= Shoot;
        _wallRunTimer.OnTimerStart -= StartWallRunningParticles;
        _wallRunTimer.OnTimerEnd -= StopWallRunningParticles;
        _wallCayoteTime.OnTimerEnd -= DetachFromWall;
        _attackTimer.OnTimerEnd -= EndDashAttack;
        _dashAttackCooldown.OnTimerStart -= DisableAttack;
        _dashAttackCooldown.OnTimerEnd -= EnableAttack;
        _dashAttackBuffer.OnTimerEnd -= ForceStartDashAttack;
    }

    private enum State{
        Moving,
        Attacking,
        Ziplining
    }
    
    private void HandleFootstepSounds()
    {
        // Check if the player is on the ground and moving horizontally.
        if (_onGround && Mathf.Abs(_rb.linearVelocity.x) > 5f || WallRunInput() && _wallSide != 0)
            _audioManager.PlayFootsteps();
        else
            _audioManager.StopFootsteps();
    }
    
    private void Update()
    {   
        switch(_playerState){
            case State.Moving:
                gameObject.tag = "Player";
                SetRunningSpriteRotation();
                SetSpriteDirection();
                Move();
                break;
            case State.Attacking:
                DashMovement();
                break;
            case State.Ziplining:
                playerSprite.eulerAngles = Vector3.zero;
                break;
        }
        //jittery camera
        SetGravity();
        UpdateTimers();
        HandleFootstepSounds();

        if(transform.position.y < killHeight) healthManager.Damage(3);
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

    private void SetSpriteDirection(){
        if(_direction == 1)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }

    private void SetRunningSpriteRotation()
    {
        if(_wallSide == 0){
            //in air or on ground
            if(_onGround){
                SlerpRotate(playerSprite, -_rb.linearVelocityX * 1.5f, 10);
            }
            else{
                SlerpRotate(playerSprite, -_rb.linearVelocityX * 0.5f, 3);
            }
        }
        else if(WallRunInput()){
            //wallrunning
            _wallRunTimer.Restart(WallRunLingerTime);
            SlerpRotate(playerSprite, _wallSide * 75, 15);
        }
        else{
            //sliding down wall
            SlerpRotate(playerSprite, _wallSide * 10, 10);
        }
    }

    private void SetAxe(){
        //axe.position = Vector3.Lerp(axe.position, transform.position + new Vector3(_directionalInput.x * 0.2f, 0), AxeFollowSpeed * Time.deltaTime);
        //axe.position = transform.position;

        if(_dashAttackCooldown.IsRunning){
            //SlerpRotate(axe, direction * -170, _axeRotationSpeed * Time.deltaTime);
            axe.Rotate(0, 0, -axeSpinningSpeed * _direction * Time.deltaTime);
        }
        else{
            //axe.eulerAngles = new Vector3(0, 0, -0.1f * _rb.linearVelocity.x);
            SlerpRotate(axe, _directionalInput.x * 30, AxeRotationSpeed * Time.deltaTime);
        }

        axeSprite.flipX =  (int) _direction == 1;
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
            _audioManager.PlaySFX(_audioManager.playerJump);
            jumpEffect.Play();
            
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpSpeed);
            animator.SetTrigger("Jump");
        }
    }

    

    private void Shoot(EmptyEventArgs args){
        Debug.Log("Shoot!");
        /*
        if(_playerState == State.Moving){
            if (TryLocateClosestTarget(out GameObject closestTarget))
            {
                Vector3 target = closestTarget.transform.position;

                if(Vector3.Distance(transform.position, target) <= attackRange){
                    //start attack
                    playerSprite.up = GetAimPosition(transform.position, target);
                    _rb.linearVelocity = playerSprite.up * shotSpeed;
                    shootEffect.Play();
                    shotTransform.up = playerSprite.up;
                }
            }
        }
        */
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
        else
            animator.SetFloat("Speed", Mathf.Abs(_rb.linearVelocityX));
        if(_directionalInput.x != 0)
            _direction = _directionalInput.x;
    }

    private void WallInteraction(){
        //if the players touching a wall
        animator.SetBool("WallRunning", false);
        if (_wallSide != 0)
        {
            if(WallRunInput()){
                //wall running
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
                _wallRunTimer.Restart(WallRunLingerTime);
                animator.SetBool("WallRunning", true);
            }
            else if(Mathf.Approximately(_directionalInput.x, -_wallSide) && Mathf.Approximately(_directionalInput.y, 1)){
                //wall jump
                _audioManager.PlaySFX(_audioManager.wallJump);
                wallJumpEffect.Play();
                _wallCayoteTime.ForceEnd();
                _rb.linearVelocity = new Vector2(_directionalInput.x * wallJumpSpeed.x, wallJumpSpeed.y);
            }
            else if (Mathf.Approximately(_directionalInput.x, _wallSide) && _rb.linearVelocityY < 0)
            {
                //sliding down wall
                //_rb.linearVelocity *= new Vector2(1, 1 - Time.deltaTime * wallSlideDrag);
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
    private void SetState(State newState)
    {
        _playerState = newState;
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
        //Debug.Log("OnGround = " + set);
        _onGround = set;
        animator.SetBool("OnGround", set);
        if (_onGround)
        { 
            _groundCayoteTime.Restart(cayoteTimeMax);
            animator.ResetTrigger("Jump");
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
    /*--------------------------*/
    /*__________HEALTH__________*/
    /*--------------------------*/
    private void OnHeal(float healthRemaining){
        //can put some particle effects here or something
        Debug.Log("Healed to " + healthRemaining);
    }
    private void OnDamaged(float damage){
        if(damage <= 0 || _invincibility.IsRunning) return;
        _audioManager.PlaySFX(_audioManager.playerHit);
        CameraScript.Shake(0.3f);
        hitEffect.Play();
        StartCoroutine(TimeController.FreezeTime(0.01f));
        _invincibility.Restart(invincibilityMax);
    }
 
    private void OnDie(GameObject gameObject)
    {
        if(deathEffect.isPlaying)return;
        //effects
        hitEffect.Play();
        deathEffect.Play();
        CameraScript.Shake(0.5f);
        //StartCoroutine(TimeController.FreezeTime(0.02f));
        StartCoroutine(DieTransition());

        //if(_checkPoint == null){
        //    _checkPoint = defaultCheckPoint;
        //}
        //transform.position = _checkPoint.transform.position;
    }

    private IEnumerator DieTransition(){
        _audioManager.PlaySFX(_audioManager.playerDeath, 0.6f);
        _playerState = State.Ziplining;
        yield return new WaitForSeconds(0.6f);
        _playerState = State.Moving;
        transition.Play();

                if (!_checkPoint) _checkPoint = defaultCheckPoint;
        RespawnHolder myRespawn = ObjectPoolManager.SpawnObject(respawnHolder, _checkPoint.transform.position, Quaternion.identity)
            .GetComponent<RespawnHolder>();
        myRespawn.Respawn(gameObject, 0f);
        
        healthManager.Reset();
        
        enemiesInRadar.Clear();
    }
    /*-------------------------------*/
    /*__________DASH ATTACK__________*/
    /*-------------------------------*/
    private void EnableAttack()
    {
        _canAttack = true;
    }

    private void DisableAttack()
    {
        _canAttack = false;
    }
    private void ForceStartDashAttack(){
        //insures that the player can attack after the buffer ends
        _dashAttackCooldown.ForceEnd();
        AttackStart(new EmptyEventArgs());
    }
    private void AttackStart(EmptyEventArgs args){
        if(_playerState == State.Moving && _canAttack){
            if (currentTarget)
            {
                _audioManager.PlaySFX(_audioManager.playerDash);
                onAttackEnable.RaiseEvent(true);
                Vector3 target = currentTarget.transform.position;
    
                if(Vector3.Distance(transform.position, target) <= attackRange){
                    //start attack
                    sliceEffect.Play();
                    SetState(State.Attacking);
                    _attackTimer.Restart(attackLength);
                    playerSprite.up = GetAimPosition(transform.position, target);
                    animator.SetBool("Attacking", true);
                }
            }
        }
        else if (!_canAttack && !_dashAttackBuffer.IsRunning && _dashAttackCooldown.RemainingSeconds < dashBufferMax)
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
        _audioManager.PlaySFX(_audioManager.enemyHit);
        damageable.Damage(playerDamage);
        
        attack.transform.up = playerSprite.up;
        
        _attackEffect.Play();
        
        //ends dash
        _attackTimer.ForceEnd();
        
        if(Physics2D.BoxCast(transform.position, new Vector2(1, 8), 0, Vector2.zero, 0, groundLayer)){
            //bounce
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 70);
            //Debug.Log("On ground!");
        }else{
            StartCoroutine(DashFollowThrough(0.005f));
        }
        
    }
    private void EndDashAttack()
    {
        _rb.linearVelocity /= AttackSlowDown;
        SetState(State.Moving);
        _dashAttackCooldown.Restart(dashAttackCooldown);
        onAttackEnable.RaiseEvent(false);
        animator.SetBool("Attacking", false);
    }
    
    IEnumerator DashFollowThrough(float time)
    {
        StartCoroutine(TimeController.FreezeTime(time));
        yield return null;
        _rb.linearVelocity = playerSprite.up * dashCompleteSpeed;
        gameObject.layer = 9;
        yield return new WaitForSeconds(0.3f);
        gameObject.layer = 0;
    }
    
    /*-------------------------------*/
    /*__________RADAR SYSTEM__________*/
    /*-------------------------------*/
    
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
        SelectEnemy(Vector2.zero);
    }

    private void RemoveEnemyFromRadar(GameObject enemy)
    {
        enemiesInRadar.Remove(enemy);
        if (enemy == currentTarget)
        {
            currentTarget = null;
        }
        if (enemy.TryGetComponent(out AbstractEnemy target))
        {
            target.PulseVisual(false);
        }
        SelectEnemy(Vector2.zero);
    }

    private void SelectEnemy(Vector2 inputDirection)
    {
        if (TryLocateDirectionalTarget(inputDirection, out GameObject directionalTarget))
        {
            if (directionalTarget.TryGetComponent(out AbstractEnemy target))
            {
                if (currentTarget && currentTarget.TryGetComponent(out AbstractEnemy current))
                {
                    current.PulseVisual(false);
                }
                target.PulseVisual(true);
            }
            currentTarget = directionalTarget;
        }
    }

    private bool TryLocateDirectionalTarget(Vector2 inputDirection, out GameObject directionalTarget)
    {
        directionalTarget = null;
        if(enemiesInRadar.Count == 0) return false;
        if(inputDirection == Vector2.zero) return TryLocateClosestTarget(out directionalTarget);
        directionalTarget = enemiesInRadar[0];
        float bestDotValue = Vector2.Dot(inputDirection, (directionalTarget.transform.position - transform.position).normalized);
        foreach (var bogie in enemiesInRadar)
        {
            float dotValue = Vector2.Dot(inputDirection, (bogie.transform.position - transform.position).normalized);
            if (dotValue > bestDotValue)
            {
                directionalTarget = bogie;
                bestDotValue = dotValue;
            }
        }

        return true;
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
    
    /*----------------------------------*/
    /*__________ZIPLINE SYSTEM__________*/
    /*----------------------------------*/
    //Remove me Later, Player is only responsible for knowing that it is on a zipline
    public void StartZipline(){
        _playerState = State.Ziplining;
        axe.localScale = new Vector3(0, 0, 0);
        ziplineEffect.Play();
        _audioManager.PlayZip();
    }

    public void EndZipline()
    {
        _audioManager.StopZip();
        axe.localScale = new Vector3(1, 1, 0);
        ziplineEffect.Stop();
        _playerState = State.Moving;
        _rb.linearVelocity = new Vector2(transform.parent.GetComponent<Rigidbody2D>().linearVelocity.x, ziplineBoost);
    }
    /*------------------------------*/
    /*__________Checkpoint__________*/
    /*------------------------------*/
    public void SetCheckpoint(GameObject checkpoint)
    {
        _audioManager.PlaySFX(_audioManager.checkPoint);
        checkpoint.GetComponent<ParticleSystem>().Play();
        killHeight = transform.position.y - 30;
        CheckPoint spawn = checkpoint.GetComponent<CheckPoint>();
        if(!_checkPoint || spawn.CheckPointIndex > _checkPoint.CheckPointIndex){
            _checkPoint = spawn;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if(!_invincibility.IsRunning)
            healthManager.Damage(1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_invincibility.IsRunning && collision.gameObject.tag == "Enemy Attack")
            healthManager.Damage(1);
    }
}