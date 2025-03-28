using System;
using DamageManagement;
using Events.Channels;
using Events.Listeners;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathBox : MonoBehaviour
    {
        [SerializeField] private GameObjectEventChannelSO checkpointEventChannel;
        [SerializeField] private int checkpointIndex = -1;

        private void Awake()
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
        }
        
        private void Start()
        {
            checkpointEventChannel.onEventRaised += SetActive;
            if (checkpointIndex > 0) gameObject.SetActive(false); //sets the deathbox inactive
        }

        private void SetActive(GameObject checkpoint)
        {
            CheckPoint cp = checkpoint.GetComponent<CheckPoint>();

            if (checkpointIndex != -1 && cp.CheckPointIndex != checkpointIndex)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HealthManager damageable = other.GetComponent<HealthManager>();
            if (damageable != null)
            {
                damageable.Damage(damageable.GetHealth());
            }
        }
    }
}