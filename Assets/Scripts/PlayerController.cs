using System;
using System.Collections.Generic;
using Events.Channels;
using Events.Input;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackLength;
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject attack;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem wallJumpEffect;
    [SerializeField] private GameObject sliceEffect;
    [SerializeField] private ParticleSystem wallRunEffect;
    [SerializeField] List<GameObject> enemiesInRadar;
    [SerializeField] private float _invincibilityMax;
    [SerializeField] private Transform axe;
    [SerializeField] private SpriteRenderer axeSprite;
    [SerializeField] private float axeSpingSpeed;

    private const float _axeFollowSpeed = 50;
    private const float _axeRotationSpeed = 300;


    private float _wallRunTimer;
    private ParticleSystem _attackEffect;
    private Vector3 _directionalInput;
    private bool _onGround;
    private Rigidbody2D _rb;
    private float _wallSide;
    private Timer _cayoteTime;
    private State _playerState;
    private float attackCounter;
    private const float attackSlowDown = 1.3f;
    private const float turningSpeed = 2;
    private const float runningSpeed = 1;
    private const float fallGravity = 14;
    private const float riseGravity = 3;
    private float _invincibility;
    public Transform _ziplineTransform;
    [SerializeField] private float ziplineLaunchSpeed;
    private float direction;

    private void Awake()
    {
        sliceEffect.SetActive(false);
        _attackEffect = attack.GetComponent<ParticleSystem>();
        inputReader.EnablePlayerActions();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _cayoteTime = new Timer(cayoteTimeMax);
        enemiesInRadar = new List<GameObject>();
        _playerState = State.Moving;
    }

    private void Start(){
        inputReader.Jump.onEventRaised += Jump;
        inputReader.Move.onEventRaised += SetDirection;
        inputReader.Attack.onEventRaised += AttackAction;
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
                Move();
                break;
            case State.Attacking:
                gameObject.tag = "Player Attack";
                Attack();
                break;
            case State.Ziplining:
                transform.position = _ziplineTransform.position;
                break;
        }
        //jittery camera

        SetSprite();
        SetGravity();

        _invincibility -= Time.deltaTime;
        _wallRunTimer -= Time.deltaTime;
    }

    private void FixedUpdate() {
        setAxe();
    }

    private void SetSprite(){
        //rotate player sprite based on their state

        if(_playerState == State.Moving){
            if(_wallSide == 0){
                //in air or on ground
                if(_onGround == true){
                    SlerpRotate(playerSprite, -_rb.linearVelocityX * 2, 10);
                }
                else{
                    SlerpRotate(playerSprite, -_rb.linearVelocityX * -1.5f, 10);
                }
            }
            else if(WallRunInput()){
                //wallrunning
                SlerpRotate(playerSprite, _wallSide * 90, 15);
                _wallRunTimer = 0.03f;
            }
            else{
                //sliding down wall
                SlerpRotate(playerSprite, _wallSide * 10, 10);
            }
        }

        wallRunParticles();
    }

    void setAxe(){
        axe.position = Vector3.Lerp(axe.position, transform.position + new Vector3(_directionalInput.x * 0.2f, 0), _axeFollowSpeed * Time.deltaTime);

        if(_playerState == State.Moving){
            SlerpRotate(axe, _directionalInput.x * 30, _axeRotationSpeed * Time.deltaTime);
        }
        else if(_playerState == State.Attacking){
            //SlerpRotate(axe, direction * -170, _axeRotationSpeed * Time.deltaTime);
            axe.Rotate(0, 0, -axeSpingSpeed * direction * Time.deltaTime);
        }

        axeSprite.flipX =  direction == 1 ? false : true;
    }

    void wallRunParticles(){
        if(_wallRunTimer < 0 && wallRunEffect.isPlaying){
            wallRunEffect.Stop();
        }

        if(_wallRunTimer > 0 && !wallRunEffect.isPlaying){
            wallRunEffect.Play();
        }
    }

    private bool WallRunInput(){
        return Mathf.Approximately(_directionalInput.x, _wallSide) && Mathf.Approximately(_directionalInput.y, 1);
    }

    private void Attack(){
        _rb.linearVelocity = playerSprite.up * attackSpeed;
        attackCounter -= Time.deltaTime;

        if(attackCounter < 0.1f){
            sliceEffect.SetActive(false);
        }

        if(attackCounter < 0){
            _rb.linearVelocity /= attackSlowDown;
            _playerState = State.Moving;
        }
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
                Vector3 target = closestTarget.transform.position;

                if(Vector3.Distance(transform.position, target) <= attackRange){
                    //start attack
                    sliceEffect.SetActive(true);
                    _playerState = State.Attacking;
                    attackCounter = attackLength;
                    playerSprite.up = AimAt(transform.position, target);
                }
            }
            
        }
    }

    private Vector3 FindClosest(string tag){
        //find all enemies
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        //make sure there is enemies
        if(targets == null){
            return Vector3.zero;
        }
        else{
            float closest = Vector3.Distance(transform.position, targets[0].transform.position);
            GameObject closestTarget = targets[0];

            //find closest
            foreach(GameObject check in targets){
                float dist = Vector3.Distance(transform.position, check.transform.position);
                if(dist < closest){
                    closest = dist;
                    closestTarget = check;
                }
            }

            return closestTarget.transform.position;
        }
    }

    private Vector3 AimAt(Vector3 a, Vector3 b){
        return new Vector3(a.x - b.x, a.y - b.y) * -1;
    }

    private void Move()
    {
        //onground vs. in air movement
        if(Turning() && _onGround){
            MoveHorizontal(turningSpeed);
        }
        else{
            MoveHorizontal(runningSpeed);
        }

        WallInteraction();
        if(!_onGround)
            _cayoteTime.Tick(Time.deltaTime);

        if(_directionalInput.x != 0)
            direction = _directionalInput.x;
    }

    private void WallInteraction(){
        //if the players touching a wall
        if(_wallSide != 0 && _onGround == false){
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
                _rb.gravityScale = _rb.linearVelocityY > 2 ? riseGravity : fallGravity;
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
        _onGround = set;
        if (_onGround)
        {
            _cayoteTime.Restart(cayoteTimeMax); 
        }
    }

    public void SetOnWall(int set){
        _wallSide = set;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Enemy") && _playerState == State.Attacking){
            attackCounter = 0.06f;
            _attackEffect.Play();
            attack.transform.up = playerSprite.up;
            TimeController.setTime(0.15f);
            StartCoroutine(TimeController.freezeTime(0.005f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GetHit(other.tag);

        if(other.tag == "Zipline"){
            _playerState = State.Ziplining;
            _ziplineTransform = other.transform;
            other.GetComponent<Zipline>().startZip();
        }
    }

    private void OnParticleCollision(GameObject other) {
        GetHit(other.tag);
    }

    private void GetHit(string tag){
        if(_invincibility < 0 && tag == "Enemy Attack"){
            hitEffect.Play();
            //TimeController.setTime(0.05f);
            StartCoroutine(TimeController.freezeTime(0.01f));
            _invincibility = _invincibilityMax;
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

    public void endZipline(Vector3 inputVelocity){
        _rb.linearVelocity = new Vector2(inputVelocity.x, 10);
        //_rb.linearVelocity = inputVelocity * ziplineLaunchSpeed;
        _playerState = State.Moving;
    }
}
