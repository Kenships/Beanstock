using System;
using System.Collections.Generic;
using Collisions;
using Unity.VisualScripting;
using Events.Channels;
using UnityEngine;
using UnityEngine.Serialization;

public class Zipline : MonoBehaviour
{
    [SerializeField] private List<Transform> _ziplinersList;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private LineRenderer vine;
    [SerializeField] private Vector3EventChannelSO endEvent;
    [SerializeField] private Transform closestPoint;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float startSpeed;
    [SerializeField] private TwoPointCollider TPCollider;
    [SerializeField] private GameObject zipHandlePrefab;
    private float _maxDist;
    private State _ziplineState;
    
    public void Awake()
    {
        _ziplinersList = new List<Transform>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TPCollider.onCollisionEnter.onEventRaised += StartZip;
        //_hitBox = gameObject.GetComponent<CircleCollider2D>();
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
                //Do nothing
                break;
            case State.Ziplining:
                //move towards end point
                for(int i = _ziplinersList.Count - 1; i >= 0; i--){
                    Transform zipliner = _ziplinersList[i];
                    
                    //zipliner.up = new Vector3(zipliner.position.x - point2.position.x, zipliner.position.y - point2.position.y) * -1;
                    
                    zipliner.GetComponent<Rigidbody2D>().linearVelocity += (Vector2)zipliner.up * (speed * Time.deltaTime);
                    zipliner.GetChild(0).transform.position = zipliner.position;
                    
                    if(Vector3.Distance(zipliner.position, point2.position) <= 1f){
                        _ziplinersList.RemoveAt(i);
                        zipliner.GetChild(0).GetComponent<ICanZipline>().EndZipline();
                        zipliner.transform.DetachChildren();
                        Destroy(zipliner.gameObject);
                    }
                    
                }
                break; 
        }

        //aim at end point
        //closestPoint.transform.up = new Vector3(closestPoint.position.x - point2.position.x, closestPoint.position.y - point2.position.y) * -1;
        
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

    Vector3 FindClosestPoint(Transform zipliner, Vector3 linePnt, Vector3 lineDir){

        //get the closest point on the line
        lineDir = Vector3.Normalize(lineDir);
        var v = zipliner.position - linePnt;
        var d = Vector3.Dot(v, lineDir);
        

        return linePnt + lineDir * d;
    }

    public void StartZip(GameObject inputObj){
        //starts when player touches the zip
        //Debug.Log("ZIP");
        if (!inputObj.TryGetComponent(out ICanZipline zipliner)) return;
        
        GameObject zipHolder = Instantiate(zipHandlePrefab, FindClosestPoint(inputObj.transform, point1.position, point2.position - point1.position), Quaternion.identity);
        Transform zipHolderTransform = zipHolder.transform;
        
        zipHolderTransform.up = new Vector3(zipHolderTransform.position.x - point2.position.x,
            zipHolderTransform.position.y - point2.position.y) * -1;
        
        zipHolder.transform.SetParent(transform.parent);
        inputObj.transform.SetParent(zipHolder.transform);
        
        _ziplinersList.Add(zipHolder.transform);
        zipliner.StartZipline();
        
        
        _ziplineState = State.Ziplining;
        zipHolder.GetComponent<Rigidbody2D>().linearVelocity = (Vector2)zipHolderTransform.up * startSpeed;
    }
}
