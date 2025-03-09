using Events.Channels;
using UnityEngine;

namespace Collisions
{
    public class CheckPointCollider : MonoBehaviour
    {
        [SerializeField] private GameObjectEventChannelSO onCheckPoint;
        [SerializeField] private int checkPointIndex;

        private void OnTriggerEnter2D(Collider2D other)
        {
            onCheckPoint.RaiseEvent(other.gameObject);
        }
    }
}
