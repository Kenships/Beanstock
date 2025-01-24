using Events.Channels;
using UnityEngine;

namespace Collisions
{
    public class PlayerRadarCollider : MonoBehaviour
    {
        [SerializeField] private RadarEventChannelSO eventChannel;
        [SerializeField] private Transform playerTransform;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.IsChildOf(playerTransform)) return;
        
            RadarInfo info = new RadarInfo()
            {
                Bogie = other.gameObject, 
                InRange = true
            };
        
            eventChannel.RaiseEvent(info);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.IsChildOf(playerTransform)) return;
        
            RadarInfo info = new RadarInfo()
            {
                Bogie = other.gameObject, 
                InRange = false
            };
        
            eventChannel.RaiseEvent(info);
        }
    }
}
