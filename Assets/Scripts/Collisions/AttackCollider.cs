using System;
using DamageManagement;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private IDamageableEventChannelSO onPlayerAttackLanded;
    [SerializeField] private GameObjectEventChannelSO onAttackGameObject;
    [SerializeField] private Transform playerTransform;
    
    public BoolEventChannelSO OnAttackEnable => onAttackEnable;
    public IDamageableEventChannelSO OnAttackIDamageable => onPlayerAttackLanded;
    public GameObjectEventChannelSO OnAttackGameObject => onAttackGameObject;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if(onAttackEnable == null)
            onAttackEnable = ScriptableObject.CreateInstance<BoolEventChannelSO>();
        if(onPlayerAttackLanded == null)
            onPlayerAttackLanded = ScriptableObject.CreateInstance<IDamageableEventChannelSO>();
        if(onAttackGameObject == null)
            onAttackGameObject = ScriptableObject.CreateInstance<GameObjectEventChannelSO>();
        if (playerTransform == null)
            playerTransform = transform.parent;
    }

    private void Start()
    {
        onAttackEnable.onEventRaised += SetTrigger;
        _collider.isTrigger = true;
        SetTrigger(false);
    }

    private void SetTrigger(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.IsChildOf(playerTransform)) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            onPlayerAttackLanded.RaiseEvent(damageable);
            OnAttackGameObject.RaiseEvent(other.gameObject);
        }
    }
}
