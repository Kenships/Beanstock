using Events.Channels;
using UnityEngine;

namespace Collisions
{
    public class PlayerWallCollider : MonoBehaviour
    {
        [SerializeField] private IntEventChannelSO groundEvent;
        [SerializeField] private Transform playerTransform;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy")) return;
            if (other.transform.IsChildOf(playerTransform)) return;
            if (other.transform.position.x > transform.position.x)
            {
                groundEvent.RaiseEvent(1);
            }
            else
            {
                groundEvent.RaiseEvent(-1);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.IsChildOf(playerTransform)) return;
            groundEvent.RaiseEvent(0);
        }
    }
}