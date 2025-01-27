using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class Archer : MonoBehaviour
{
    public Transform player;
    public float visionDistance;
    public Transform forwardTip;

    public GameObject arrow;
    public Transform bulletPos;
    private float timer;
    
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
            Timer();
            // doesn't reset timer when exiting range
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

    private void Timer()
    {
        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            timer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(arrow, bulletPos.position, Quaternion.identity);    

    }


}
