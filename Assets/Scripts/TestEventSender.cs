using System.Collections;
using Events.Channels;
using UnityEngine;

public class TestEventSender : MonoBehaviour
{
    [SerializeField] private StringEventChannelSO testEvent;

    private void Start()
    {
        StartCoroutine(PrintCoroutine("My name is " + name));
    }
    
    private IEnumerator PrintCoroutine(string message)
    {
        Debug.Log(message);
        yield return null;
        testEvent.RaiseEvent(gameObject.name);
    }
}
