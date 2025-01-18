using UnityEngine;
using UnityEngine.Events;

namespace Events.Channels
{
    public abstract class AbstractEventChannelSO<T> : ScriptableObject
    {
        [Tooltip("Assign event to this channel | added subscribers will be invoked.")]
        public UnityAction<T> onEventRaised;

        public virtual void RaiseEvent(T type)
        {
            onEventRaised?.Invoke(type);
        }
    }
}