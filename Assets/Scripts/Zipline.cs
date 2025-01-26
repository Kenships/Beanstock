using System;
using Unity.VisualScripting;
using Events.Channels;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] LineRenderer _vine;
    [SerializeField] private Vector3EventChannelSO endEvent;
    private float _maxDist;
    [SerializeField] private Transform closestPoint;
    [SerializeField] private Rigidbody2D _rb;
    private State _ziplineState;
    [SerializeField] private float speed;
    private float _delayPeriod;
    private CircleCollider2D _hitBox;
    [SerializeField] private float startSpeed;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hitBox = gameObject.GetComponent<CircleCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _vine.SetPosition(0, point1.position);
        _vine.SetPosition(1, point2.position);
        _maxDist = Vector3.Magnitude(getLineDirection());
        _ziplineState = State.Idle;
    }

    private enum State{
        Idle,
        Ziplining,
        Recovering
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (_ziplineState){
            case State.Idle:
                //got to closest position to player on line
                closestPoint.position = findClosestPoint(point1.position, getLineDirection());
                _rb.linearVelocity *= 0;
                break;
            case State.Ziplining:
                //move towards end point
                _rb.linearVelocity += (Vector2)closestPoint.up * speed * Time.deltaTime;
                break;
        }

        //aim at end point
        closestPoint.transform.up = new Vector3(closestPoint.position.x - point2.position.x, closestPoint.position.y - point2.position.y) * -1;
        resetPosition(point1.position, point2.position, true);
        resetPosition(point2.position, point1.position, false);

        //turn off hitbox once player leaves
        _delayPeriod -= Time.deltaTime;

        if(_delayPeriod > 0 || _ziplineState == State.Ziplining){
            _hitBox.enabled = false;
        }
        else{
            _hitBox.enabled = true;
        }
    }

    private void resetPosition(Vector3 a, Vector3 b, bool endPoint){
        //reset when at end points
        if(Vector3.Distance(closestPoint.position, a) >= _maxDist){
            closestPoint.position = b;

            //end zipline
            if(endPoint && _ziplineState == State.Ziplining){
                end();
            }
        }
    }

    void end(){
        //finish zipline
        _ziplineState = State.Idle;
        _delayPeriod = 0.5f;
        endEvent.RaiseEvent(_rb.linearVelocity);
    }

    Vector3 getLineDirection(){
        return point2.position - point1.position;
    }

    Vector3 findClosestPoint(Vector3 linePnt, Vector3 lineDir){

        //get the closest point on the line
        lineDir = Vector3.Normalize(lineDir);
        var v = playerTransform.position - linePnt;
        var d = Vector3.Dot(v, lineDir);


        return linePnt + lineDir * d;
    }

    public void startZip(){
        //starts when player touches the zip
        _ziplineState = State.Ziplining;
        _rb.linearVelocity = (Vector2)closestPoint.up * startSpeed;
    }
}
