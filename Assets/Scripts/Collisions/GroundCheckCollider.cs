using System;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Collisions
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GroundCheckCollider : MonoBehaviour
    {
        [SerializeField] private BoolEventChannelSO groundEvent;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool isGrounded;
        public BoolEventChannelSO GroundEvent => groundEvent;
        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            if(groundEvent == null)
                groundEvent = ScriptableObject.CreateInstance<BoolEventChannelSO>();
            if(parentTransform == null)
                parentTransform = transform.parent;
        }

        private void Start()
        {
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.IsChildOf(parentTransform)) return;
            groundEvent.RaiseEvent(true);
            isGrounded = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (isGrounded) return;
            
            groundEvent.RaiseEvent(true);
            isGrounded = true;
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.transform.IsChildOf(parentTransform)) return;
            groundEvent.RaiseEvent(false);
            isGrounded = false;
        }
    }
}
