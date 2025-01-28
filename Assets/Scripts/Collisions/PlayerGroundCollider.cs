using Events.Channels;
using UnityEngine;

namespace Collisions
{
    public class PlayerGroundCollider : MonoBehaviour
    {
        [SerializeField] private BoolEventChannelSO groundEvent;
        [SerializeField] private Transform playerTransform;
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.IsChildOf(playerTransform)) return;
            groundEvent.RaiseEvent(true);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.transform.IsChildOf(playerTransform)) return;
            groundEvent.RaiseEvent(false);
        }
    }
}
