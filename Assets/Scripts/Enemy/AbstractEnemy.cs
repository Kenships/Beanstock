using System.Collections;
using System.Collections.Generic;
using Collisions;
using Events.Channels;
using UnityEngine;
using Util;

namespace Enemy
{
    [RequireComponent(typeof(HealthManager))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractEnemy : MonoBehaviour
    {
    
        [Header("___RESPAWN___")]
        [SerializeField] protected GameObject respawnHolder;
        protected Vector3 _originalPosition;
    
        [Header("___ENEMY STATS___")]
        [SerializeField] protected float attackDamage;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float attackSpeed;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected float attackDuration;
    
        [Header("___ENEMY REFERENCES___")]
        
        [SerializeField] protected RadarCollider radarCollider;
        [SerializeField] protected GroundCheckCollider groundCheckCollider;
        protected Rigidbody2D _rb;
        protected HealthManager _healthManager;
    
        [Header("___ENEMY VISUALS___")]
        [SerializeField] protected SpriteRenderer enemySprite;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Material matFlash;
        [SerializeField] protected Material matOriginal;
        protected Color _originalColor;
    
        [Header("___Knockback Config___")]
        [SerializeField] protected float hitSpeed = 100f;
        [SerializeField] protected float hitDrag = 10f;
        [SerializeField] protected float flashTime = 0.3f;

        [Header("___RADAR DEBUG___")] 
        [SerializeField] protected float radarRadius = 10f;
        [SerializeField] protected List<GameObject> inRangeBogies;
    
        protected Timer AttackCooldownTimer;
        protected Timer AttackDurationTimer;
        protected bool IsGrounded;
    
    
        protected void Awake()
        {
            AttackCooldownTimer = new Timer(attackCooldown);
            AttackDurationTimer = new Timer(attackDuration);
        
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _healthManager = GetComponent<HealthManager>();
        }

        protected void Start()
        {
            
            _healthManager.OnDamage.onEventRaised += OnHit;
            _healthManager.OnHeal.onEventRaised += OnHeal;
            _healthManager.OnDie.onEventRaised += OnDeath;
            _healthManager.OnRespawn.onEventRaised += OnRespawn;
        
            if (radarCollider == null) Debug.LogWarning("RadarCollider is not assigned to " + gameObject.name);
            else radarCollider.RadarChannel.onEventRaised += ProcessBogie;
        
            if (groundCheckCollider == null) Debug.LogWarning("GroundCheckCollider is not assigned to " + gameObject.name);
            else groundCheckCollider.GroundEvent.onEventRaised += SetGrounded;

            _originalPosition = transform.position;
        
            _originalColor = enemySprite.color;
            
            radarCollider.SetRadius(radarRadius);
        }

        protected virtual void ProcessBogie(RadarInfo radarInfo)
        {
            if (!radarInfo.Bogie.CompareTag("Player")) return;
        
            if(radarInfo.InRange)
                AddBogie(radarInfo.Bogie);
            else
                RemoveBogie(radarInfo.Bogie);
        }
    
        protected void AddBogie(GameObject bogie)
        {
            inRangeBogies.Add(bogie);
        }
        protected void RemoveBogie(GameObject bogie)
        {
            inRangeBogies.Remove(bogie);
        }
        private void SetGrounded(bool isGrounded)
        {
            IsGrounded = isGrounded;
        }
        protected bool TryGetPlayer(out GameObject player)
        {
            if (inRangeBogies.Count == 0)
            {
                player = null;
                return false;
            }

            foreach (var bogey in inRangeBogies)
            {
                if (bogey.CompareTag("Player"))
                {
                    player = bogey;
                    return true;
                }
            }
        
            player = null;
            return false;
        }

        protected virtual void OnHit(float healthRemaining)
        {
            StartCoroutine(GetHit());
        }

        protected virtual void OnHeal(float healthRemaining)
        {
        
        }

        protected virtual void OnDeath(GameObject deadEnemy)
        {
            RespawnHolder myRespawn =
                ObjectPoolManager.SpawnObject(respawnHolder, _originalPosition, Quaternion.identity).GetComponent<RespawnHolder>();
            myRespawn.Respawn(gameObject);
            inRangeBogies.Clear();
            _healthManager.Reset();
            
        }

        protected virtual void OnRespawn(GameObject gameObject)
        {
            enemySprite.material = matOriginal;
            enemySprite.color = _originalColor;
        }
    
        private Vector3 GetAimPosition(Vector3 a, Vector3 b){
            return new Vector3(a.x - b.x, a.y - b.y) * -1;
        }
    
        protected virtual IEnumerator GetHit()
        {
            //knock back
            if (TryGetPlayer(out var player))
            {
                Debug.Log("knockback");
                transform.up = GetAimPosition(transform.position, player.transform.position) * -1;
                _rb.linearVelocity = transform.up * hitSpeed;
                transform.up = Vector3.zero;
            }
        
            //end of knockback

            enemySprite.material = matFlash;
            enemySprite.color = Color.white;

            for(float i = 0; i < flashTime; i += Time.deltaTime){
                yield return null;
                _rb.linearVelocity *= 1 - Time.deltaTime * hitDrag;
            }
            //yield return new WaitForSeconds(_flashTime)

            enemySprite.color = _originalColor;
            enemySprite.material = matOriginal;
        }
    }
}
