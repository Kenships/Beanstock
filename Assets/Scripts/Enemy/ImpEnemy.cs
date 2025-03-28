using System;
using System.Collections.Generic;
using Collisions;
using DamageManagement;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class ImpEnemy : AbstractEnemy
    {
        [Header("___Imp Config___")]
        [SerializeField] private Vector3[] patrolPosition;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private int moveRange;
        [SerializeField] private float climbSpeed;
        [SerializeField] private float jumpSpeed;
        //[SerializeField] private Animator animator;
        [Header("___Attack Radar Debug___")]
        [SerializeField] private State enemyState;
        [SerializeField] private RadarCollider attackRadar;
        [SerializeField] private List<GameObject> inAttackRangeBogies;
        private int _aimDirection;
        private bool _onWall;
        private int _patrolNum;
        private Transform player;
        private enum State{
            Patrolling,
            Attacking,
            Chasing
        }
        
        private new void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            for(int i = 0; i < patrolPosition.Length; i++){
                patrolPosition[i] = new Vector3(transform.position.x + 5 - i * 10, transform.position.y);
            }
            base.Start();
            AttackCooldownTimer.Cancel();
            attackRadar.SetRadius(attackRange);
            attackRadar.RadarChannel.onEventRaised += ProcessBogieInAttackRange;
            AttackDurationTimer.OnTimerEnd += AttemptAttack;
        }

        private void OnDisable()
        {
            inAttackRangeBogies.Clear();
            inRangeBogies.Clear();
        }

        private void ProcessBogieInAttackRange(RadarInfo radarInfo)
        {
            if (!radarInfo.Bogie.CompareTag("Player")) return;

            if (radarInfo.InRange)
            {
                inAttackRangeBogies.Add(radarInfo.Bogie);
                enemyState = State.Attacking;
            }
            else
            {
                inAttackRangeBogies.Remove(radarInfo.Bogie);
                AttackDurationTimer.Cancel();
            }
                
        }

        protected override void ProcessBogie(RadarInfo radarInfo)
        {
            if (!radarInfo.Bogie.CompareTag("Player")) return;

            if (radarInfo.InRange)
                AddBogie(radarInfo.Bogie);
            else
                RemoveBogie(radarInfo.Bogie);
        }

        // Update is called once per frame
        void Update()
        {
            switch (enemyState){
                case State.Patrolling:

                    Move(patrolPosition[_patrolNum]);

                    if(Vector3.Distance(transform.position, patrolPosition[_patrolNum]) < 1){
                        _patrolNum ++;
                        if(_patrolNum >= patrolPosition.Length){
                            _patrolNum = 0;
                        }
                    }

                    if(inRangeBogies.Count > 0 && Vector3.Distance(transform.position, _originalPosition) < moveRange){
                        enemyState = State.Chasing;
                    }
                    break;
                case State.Chasing:
                    if(inRangeBogies.Count == 0 || Vector3.Distance(transform.position, _originalPosition) > moveRange){
                        enemyState = State.Patrolling;
                    }
                    else
                    {
                        Move(inRangeBogies[0].transform.position);
                    }
                    
                    break;
                case State.Attacking:
                    StartAttack();
                    AttackDurationTimer.Tick(Time.deltaTime);
                    break;
            }
            AttackCooldownTimer.Tick(Time.deltaTime);
            if(_rb.linearVelocity.x > 0){
                enemySprite.flipX = false;
            }
            else{
                enemySprite.flipX = true;
            }
        }

        void StartAttack(){
            if (!AttackDurationTimer.IsRunning)
            {
                AttackDurationTimer.Restart(attackDuration);
            }
        }

        private void AttemptAttack()
        {
            if(inAttackRangeBogies.Count > 0 && !AttackCooldownTimer.IsRunning){
                if(Vector3.Distance(transform.position, player.position) < 3){
                    AttackObject(inAttackRangeBogies[0]);
                }
                AttackCooldownTimer.Restart(attackCooldown);
            }
            else
            {
                enemyState = State.Chasing;
            }
        }

        private void AttackObject(GameObject inAttackRangeBogey)
        {
            if (inAttackRangeBogey.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(attackDamage);
                if(animator != null){
                    animator.SetTrigger("Attack");
                }
            }
        }

        void Move(Vector3 aimPosition){
            _onWall = Physics2D.BoxCast(transform.position, new Vector2(2, 0.8f), 0, Vector2.zero, 0, groundLayer);

            //aim at target (x), and move
            _aimDirection = transform.position.x < aimPosition.x ? 1 : -1;
            _rb.linearVelocity += new Vector2(moveSpeed * _aimDirection, 0) * Time.deltaTime;

            //climb
            if(transform.position.y < aimPosition.y && _onWall){
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, climbSpeed);
            }

            //if the player is a lot higher, jump towards them
            if(IsGrounded && transform.position.y + 5 < aimPosition.y){
                Jump();
            }
        }

        void Jump(){
            //_rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpSpeed);
        }
    }
}