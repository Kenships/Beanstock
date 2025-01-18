using Events.Channels;
using UnityEngine;
using UnityEngine.Events;

namespace Events.Listeners
{
    public abstract class AbstractEventChannelListener<TEventChannel, TEventType> : 
        MonoBehaviour where TEventChannel : AbstractEventChannelSO<TEventType>
    {
        [SerializeField] protected TEventChannel eventChannel;
        [SerializeField] protected UnityEvent<TEventType> response;

        protected virtual void OnEnable()
        {
            if (eventChannel != null)
            {
                eventChannel.onEventRaised += OnEventRaised;
            }
        }

        protected virtual void OnDisable()
        {
            if (eventChannel != null)
            {
                eventChannel.onEventRaised -= OnEventRaised;
            }
        }

        public void OnEventRaised(TEventType eventType)
        {
            response?.Invoke(eventType);
        }
    
    }
}