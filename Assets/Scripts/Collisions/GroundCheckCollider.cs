using System;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Collisions
{
    public class GroundCheckCollider : MonoBehaviour
    {
        [SerializeField] private BoolEventChannelSO groundEvent;
        [SerializeField] private Transform parentTransform;
        public BoolEventChannelSO GroundEvent => groundEvent;

        private void Awake()
        {
            if(groundEvent == null)
                groundEvent = ScriptableObject.CreateInstance<BoolEventChannelSO>();
            if(parentTransform == null)
                parentTransform = transform.parent;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.IsChildOf(parentTransform)) return;
            groundEvent.RaiseEvent(true);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.transform.IsChildOf(parentTransform)) return;
            groundEvent.RaiseEvent(false);
        }
    }
}
