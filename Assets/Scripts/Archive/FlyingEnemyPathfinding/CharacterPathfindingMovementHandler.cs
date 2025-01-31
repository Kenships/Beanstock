using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Numerics;

public class CharacterPathfindingMovementHandler : MonoBehaviour
{
    public float speed = 20f;
    private int currentPathIndex;
    private List<UnityEngine.Vector3> path;

    public UnityEngine.Vector3 GetPosition()
    {
        return transform.position;
    }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HandleMovement()
    {
        if (path != null)
        {
            UnityEngine.Vector3 targetPosition = path[currentPathIndex];
            if (UnityEngine.Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                UnityEngine.Vector3 moveDir = (targetPosition - transform.position).normalized;
                rb.linearVelocity = moveDir * speed;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    path = null;
                    rb.linearVelocity = UnityEngine.Vector3.zero;
                }
    
            }
        }
        else
        {
            rb.linearVelocity = UnityEngine.Vector3.zero;
        }
    }
    
    public void SetTargetPosition(UnityEngine.Vector3 targetPosition)
    {
        currentPathIndex = 0;
        path = FlyingEnemyPathfinding.Instance.FindPath(GetPosition(), targetPosition);
        if (path != null && path.Count > 1)
        {
            path.RemoveAt(0);
        }
    }
    

}