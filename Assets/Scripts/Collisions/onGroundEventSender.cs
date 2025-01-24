using UnityEngine;
using Events.Channels;

public class onGroundEventSender : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO groundEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        groundEvent.RaiseEvent(true);
    }

    private void OnTriggerExit2D(Collider2D other) {
        groundEvent.RaiseEvent(false);
    }
}
