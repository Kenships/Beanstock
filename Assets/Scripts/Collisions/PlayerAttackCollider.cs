using System;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttackCollider : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO onAttackEnable;
    [SerializeField] private GameObjectEventChannelSO onHit;

    public void Start()
    {
        onAttackEnable.onEventRaised += EnableTrigger;
    }

    private void EnableTrigger(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
