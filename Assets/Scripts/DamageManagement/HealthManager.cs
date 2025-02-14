using System;
using DamageManagement;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private FloatEventChannelSO onDamage;
    [SerializeField] private FloatEventChannelSO onHeal;
    [SerializeField] private FloatEventChannelSO onHealthSet;
    [SerializeField] private GameObjectEventChannelSO onDie;
    public FloatEventChannelSO OnDamage => onDamage;
    public FloatEventChannelSO OnHeal => onHeal;
    public FloatEventChannelSO OnHealthSet => onHealthSet;
    public GameObjectEventChannelSO OnDie => onDie;
    private float _health;

    private void Awake()
    {
        _health = maxHealth;
        if(onDamage == null)
            onDamage = ScriptableObject.CreateInstance<FloatEventChannelSO>();
        if (onHeal == null)
            onHeal = ScriptableObject.CreateInstance<FloatEventChannelSO>();
        if (onHealthSet == null)
            onHealthSet = ScriptableObject.CreateInstance<FloatEventChannelSO>();
        if (onDie == null)
            onDie = ScriptableObject.CreateInstance<GameObjectEventChannelSO>();
    }
    public virtual void Die()
    {
        onDie.onEventRaised?.Invoke(gameObject);
    }
    public void Damage(float damage)
    {
        _health -= damage;
        onDamage.onEventRaised?.Invoke((int)_health);
        if (_health <= 0f)
        {
            _health = 0f;
            Die();
        }
        
    }

    public void Reset()
    {
        _health = maxHealth;
    }
    public void Heal(float heal)
    {
        _health += heal;
        if (_health > maxHealth)
            _health = maxHealth;
        onHeal.onEventRaised?.Invoke(_health);
    }
    
    public void SetHealth(float health)
    {
        _health = health;
        onHealthSet.onEventRaised?.Invoke(_health);
    }
    
    public float GetHealth()
    {
        return _health;
    }
}
