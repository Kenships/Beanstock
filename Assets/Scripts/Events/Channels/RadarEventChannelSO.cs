using UnityEngine;

namespace Events.Channels
{
    [CreateAssetMenu(fileName = "RadarEventChannelSO", menuName = "Events/RadarEventChannelSO")]
    public class RadarEventChannelSO : AbstractEventChannelSO<RadarInfo>
    {
    
    }

    public struct RadarInfo
    {
        public GameObject Bogie;
        public bool InRange;
    }
}