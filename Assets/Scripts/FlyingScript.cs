using UnityEngine;

public class FlyingScript : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _originalPosition;
    [SerializeField] private float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveTowards(_originalPosition);
    }

    void moveTowards(Vector3 position){
        transform.up = new Vector3(transform.position.x - position.x, transform.position.y - position.y);
        _rb.linearVelocity += (Vector2)transform.up * moveSpeed * Time.deltaTime;
        transform.up = Vector2.zero;
    }
}
