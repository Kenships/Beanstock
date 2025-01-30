using DamageManagement;
using UnityEngine;

namespace Events.Channels
{
    [CreateAssetMenu(fileName = "IDamageableEventChannelSO", menuName = "Events/IDamageableEventChannelSO")]
    public class IDamageableEventChannelSO : AbstractEventChannelSO<IDamageable>
    {
    
    }
}