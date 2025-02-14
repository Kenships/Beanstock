using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Collisions
{
    public class RadarCollider : MonoBehaviour
    {
        [SerializeField] private RadarEventChannelSO eventChannel;
        [SerializeField] private Transform parentTransform;
        public RadarEventChannelSO RadarChannel => eventChannel;
        private void Awake()
        {
            if(eventChannel == null)
                eventChannel = ScriptableObject.CreateInstance<RadarEventChannelSO>();
            if(parentTransform == null)
                parentTransform = transform.parent;
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
