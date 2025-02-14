using System;
using System.Collections.Generic;
using Collisions;
using Events.Channels;
using UnityEngine;
using Util;

public abstract class AbstractEnemy : MonoBehaviour
{
    [Header("___ENEMY STATS___")]
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDuration;
    [Header("___ENEMY REFERENCES___")]
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private RadarCollider radarCollider;
    [SerializeField] private GroundCheckCollider groundCheckCollider;
    [SerializeField] private GameObject enemySprite;
    [Header("___RADAR DEBUG___")]
    [SerializeField] private List<GameObject> inRangeBogies;
    
    protected Timer AttackCooldownTimer;
    protected Timer AttackDurationTimer;
    protected bool IsGrounded;
    
    private void Awake()
    {
        AttackCooldownTimer = new Timer(attackCooldown);
        AttackDurationTimer = new Timer(attackDuration);
    }

    private void Start()
    {
        if (healthManager == null) Debug.LogWarning("HealthManager is not assigned to " + gameObject.name);
        else
        {
            healthManager.OnDamage.onEventRaised += OnHit;
            healthManager.OnHeal.onEventRaised += OnHeal;
        }
        
        if (radarCollider == null) Debug.LogWarning("RadarCollider is not assigned to " + gameObject.name);
        else radarCollider.RadarChannel.onEventRaised += ProcessBogie;
        
        if (groundCheckCollider == null) Debug.LogWarning("GroundCheckCollider is not assigned to " + gameObject.name);
        else groundCheckCollider.GroundEvent.onEventRaised += SetGrounded;
        
    }

    public abstract void AttackStart();
    
    protected abstract void ProcessBogie(RadarInfo radarInfo);
    
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
    
    protected abstract void OnHit(float healthRemaining);
    protected abstract void OnHeal(float healthRemaining);
    
    protected abstract void OnDeath(GameObject deadEnemy);
}
