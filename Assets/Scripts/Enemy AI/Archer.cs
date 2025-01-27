using UnityEngine;
using UnityEngine.Video;

public class Archer : MonoBehaviour
{
    public Transform player;
    public float visionDistance;
    public Transform forwardTip;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < visionDistance)
        {
            LookAtPlayer();
        }
    }

    Vector3 GetDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }

    private void LookAtPlayer()
    {
        Vector3 direction = GetDirectionToPlayer();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }
}
