using UnityEngine;

namespace Events.Channels
{
    [CreateAssetMenu(fileName = "VoidEventChannelSO", menuName = "Events/VoidEventChannelSO")]
    public class VoidEventChannelSO : AbstractEventChannelSO<EmptyEventArgs>
    {
    
    }

    public struct EmptyEventArgs
    {
    
    }
}