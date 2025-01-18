
using Events.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float moveSpeed;
    private Vector3 moveDir;
    
    private void Awake()
    {
        inputReader.EnablePlayerActions();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (moveDir.sqrMagnitude > 0.01f)
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        Debug.Log(dir);
        moveDir = dir;
    }
}
