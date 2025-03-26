using Events.Channels;
using UnityEngine;

namespace DamageManagement
{
    public class HealthManager : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 1f;
        [SerializeField] private FloatEventChannelSO onDamage;
        [SerializeField] private FloatEventChannelSO onHeal;
        [SerializeField] private FloatEventChannelSO onHealthSet;
        [SerializeField] private GameObjectEventChannelSO onDie;
        [SerializeField] private GameObjectEventChannelSO onRespawn;
        public FloatEventChannelSO OnDamage => onDamage;
        public FloatEventChannelSO OnHeal => onHeal;
        public FloatEventChannelSO OnHealthSet => onHealthSet;
        public GameObjectEventChannelSO OnDie => onDie;
        public GameObjectEventChannelSO OnRespawn => onRespawn;
        [SerializeField] private float health;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if(onDamage == null)
                onDamage = ScriptableObject.CreateInstance<FloatEventChannelSO>();
            if (onHeal == null)
                onHeal = ScriptableObject.CreateInstance<FloatEventChannelSO>();
            if (onHealthSet == null)
                onHealthSet = ScriptableObject.CreateInstance<FloatEventChannelSO>();
            if (onDie == null)
                onDie = ScriptableObject.CreateInstance<GameObjectEventChannelSO>();
            if (onRespawn == null)
                onRespawn = ScriptableObject.CreateInstance<GameObjectEventChannelSO>();

            health = maxHealth;
        }
        public void Die()
        {
            onDie.onEventRaised?.Invoke(gameObject);
        }
        public void Damage(float damage)
        {
            SetHealth(health - damage);
            onDamage.onEventRaised?.Invoke(health);
            if (health <= 0f)
            {
                health = 0f;
                Die();
            }
        
        }

        public void Reset()
        {
            SetHealth(maxHealth);
            onRespawn.onEventRaised?.Invoke(gameObject);
            
        }
        public void Heal(float heal)
        {
            SetHealth(health + heal);
            if (health > maxHealth)
                health = maxHealth;
            onHeal.onEventRaised?.Invoke(health);
        }
    
        public void SetHealth(float health)
        {
            this.health = health;
            onHealthSet.onEventRaised?.Invoke(this.health);
        }
    
        public float GetHealth()
        {
            return health;
        }
    }
}
