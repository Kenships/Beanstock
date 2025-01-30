using System;
using DamageManagement;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttackCollider : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private IDamageableEventChannelSO onPlayerAttackLanded;
    [SerializeField] private Transform playerTransform;
    public void Start()
    {
        onAttackEnable.onEventRaised += SetTrigger;
        SetTrigger(false);
    }

    private void SetTrigger(bool value)
    {
        Debug.Log(value);
        gameObject.SetActive(value);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.IsChildOf(playerTransform)) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            onPlayerAttackLanded.RaiseEvent(damageable);
        }
    }
}
