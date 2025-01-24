using UnityEngine;
using Events.Channels;

public class OnWallEventSender : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO groundEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
                if(other.transform.position.x > transform.position.x){
            groundEvent.RaiseEvent(1);
        }
        else{
            groundEvent.RaiseEvent(-1);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        groundEvent.RaiseEvent(0);
    }
}
