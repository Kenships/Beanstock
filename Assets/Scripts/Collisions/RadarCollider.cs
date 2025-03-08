using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Collisions
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class RadarCollider : MonoBehaviour
    {
        [SerializeField] private RadarEventChannelSO eventChannel;
        [SerializeField] private Transform parentTransform;
        public RadarEventChannelSO RadarChannel => eventChannel;
        private CircleCollider2D _collider;
        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            if(eventChannel == null)
                eventChannel = ScriptableObject.CreateInstance<RadarEventChannelSO>();
            if(parentTransform == null)
                parentTransform = transform.parent;
            _collider.isTrigger = true;
        }

        public void SetRadius(float radius)
        {
            _collider.radius = radius;
            _collider.isTrigger = true;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.IsChildOf(parentTransform)) return;
        
            RadarInfo info = new RadarInfo()
            {
                Bogie = other.gameObject, 
                InRange = true
            };
        
            eventChannel.RaiseEvent(info);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.IsChildOf(parentTransform)) return;
        
            RadarInfo info = new RadarInfo()
            {
                Bogie = other.gameObject, 
                InRange = false
            };
        
            eventChannel.RaiseEvent(info);
        }
    }
}
