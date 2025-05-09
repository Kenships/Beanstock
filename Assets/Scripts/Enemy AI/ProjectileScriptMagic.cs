using System.Numerics;
using System.Threading;
using UnityEngine;

public class ProjectileScriptMagic : MonoBehaviour
{
    public float force;
    public string playerName;
    public GameObject areaOfEffect;
    private GameObject player;
    private Rigidbody2D rb;
    private float timer;


    private void Start()
    {
        // shoot towards player
        player = GameObject.Find(playerName);
        UnityEngine.Vector3 direction = player.transform.position - transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new UnityEngine.Vector2(direction.x, direction.y).normalized * force;


        //rotate to face player
        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, rot);

    }

    private void Update()
    {
        Timer();
    }

    private void Timer()
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.layer == 3)
        {
            Instantiate(areaOfEffect, transform.position, other.transform.rotation);
            Destroy(gameObject);
        }
    }
   
}