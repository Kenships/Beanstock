using System;
using Unity.VisualScripting;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class Zipline : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private LineRenderer vine;
    [SerializeField] private Vector3EventChannelSO endEvent;
    [SerializeField] private Transform closestPoint;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float startSpeed;
    
    private float _maxDist;
    private State _ziplineState;
    //private CircleCollider2D _hitBox;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_hitBox = gameObject.GetComponent<CircleCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        vine.SetPosition(0, point1.position);
        vine.SetPosition(1, point2.position);
        _maxDist = Vector3.Magnitude(GetLineDirection());
        _ziplineState = State.Idle;
    }

    private enum State{
        Idle,
        Ziplining,
        Recovering
    }

    // Update is called once per frame
    void Update()
    {
        switch (_ziplineState){
            case State.Idle:
                //got to closest position to player on line
                closestPoint.position = FindClosestPoint(point1.position, GetLineDirection());
                //rb.linearVelocity *= 0;
                break;
            case State.Ziplining:
                //move towards end point
                rb.linearVelocity += (Vector2)closestPoint.up * (speed * Time.deltaTime);
                playerTransform.position = closestPoint.position;
                break; 
        }

        //aim at end point
        closestPoint.transform.up = new Vector3(closestPoint.position.x - point2.position.x, closestPoint.position.y - point2.position.y) * -1;
        ResetPosition(point1.position, point2.position, true);
        ResetPosition(point2.position, point1.position, false);
    }

    private void ResetPosition(Vector3 a, Vector3 b, bool endPoint){
        //reset when at end points
        if(Vector3.Distance(closestPoint.position, a) >= _maxDist){
            closestPoint.position = b;

            //end zipline
            if(endPoint && _ziplineState == State.Ziplining){
                End();
            }
        }
    }

    private void End(){
        //finish zipline
        _ziplineState = State.Idle;
        endEvent.RaiseEvent(rb.linearVelocity);
    }

    private Vector3 GetLineDirection(){
        return point2.position - point1.position;
    }

    Vector3 FindClosestPoint(Vector3 linePnt, Vector3 lineDir){

        //get the closest point on the line
        lineDir = Vector3.Normalize(lineDir);
        var v = playerTransform.position - linePnt;
        var d = Vector3.Dot(v, lineDir);
        

        return linePnt + lineDir * d;
    }

    public void StartZip(){
        //starts when player touches the zip
        
        _ziplineState = State.Ziplining;
        rb.linearVelocity = (Vector2)closestPoint.up * startSpeed;
    }
}
