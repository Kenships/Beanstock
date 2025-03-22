using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using DamageManagement;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private int _moveDirection;
    [SerializeField] private float moveSpeed;
    private float _attackCounter;
    [SerializeField] private BossArm leftArm;
    [SerializeField] private BossArm rightArm;
    [SerializeField] private HealthManager head;
    private float _health;
    private int _roarCounter;
    [SerializeField] private GameObject roarBlast;
    [SerializeField] private Animation headSwing;
    [SerializeField] private GameObject deathEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _health = head.GetHealth();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(MovementPattern(1));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(moveSpeed * _moveDirection, 0) * Time.deltaTime;
        _attackCounter += Time.deltaTime;

        if(_attackCounter > 2){
            _attackCounter = 0;

            if(Random.Range(1, 6) == 1){
                //head swing
                headSwing.Play();
            }
            else if(Random.Range(1, 5) == 1){
                //clap with both arms
                leftArm.startClap();
                rightArm.startClap();
                
            }
            else{
                //arm slams
                if(Random.Range(1, 3) == 1){
                    leftArm.startSlam();
                }
                else{
                    rightArm.startSlam();
                }
            }
        }

        //getting hit
        if(_health != head.GetHealth()){
            if(_health == 1){
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            _health = head.GetHealth();
            _roarCounter++;

            //do the roar
            if(_roarCounter >= 3){
                StartCoroutine(startRoar());
                _roarCounter = 0;
            }
        }
    }
    
    IEnumerator startRoar(){
        yield return new WaitForSeconds(0.15f);
        Instantiate(roarBlast, transform.position + new Vector3(0, 15), Quaternion.identity);
    }

    IEnumerator MovementPattern(int currentDirection){
        Debug.Log(currentDirection);
        _moveDirection = currentDirection;
        yield return new WaitForSeconds(3);
        _moveDirection = 0;
        yield return new WaitForSeconds(1);
        StartCoroutine(MovementPattern(currentDirection * - 1));
    }
}
