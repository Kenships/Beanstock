using System;
using Events.Channels;
using UnityEngine;

public class PlayerEventListener : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO playerGroundChannel;
    [SerializeField] private IntEventChannelSO playerWallChannel;
    [SerializeField] private RadarEventChannelSO radar;
    [SerializeField] private IDamageableEventChannelSO playerAttackLanded;
    [SerializeField] public PlayerController player;
    public void Start()
    {
        playerGroundChannel.onEventRaised += player.SetOnGround;
        playerWallChannel.onEventRaised += player.SetOnWall;
        radar.onEventRaised += player.ProcessBogie;
        playerAttackLanded.onEventRaised += player.OnAttackLanded;
    }
}
