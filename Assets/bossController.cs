using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class bossController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _moveDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDistance;
    private float _attackCounter;
    private float _groundPountTimer;
    private Transform _attackArm;
    [SerializeField] private Transform[] arms;
    [SerializeField] private GameObject[] armHitBoxes;
    private int attackSide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(MoveTime(1));
        _attackArm = arms[0];
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        _rb.linearVelocity = new Vector2(_moveDirection * moveSpeed, 0) * Time.deltaTime;
        _attackCounter += Time.deltaTime;

        /*
        Attacks:
         Roar: giant does this when the player hits the head multiple times in succession - blasts player away
         1 arm ground pound: creates shock wave on ground + damages player if they are touching that arm
         2 arm slam: crushes the player between the two arms


        2 phases: phase 2 is a faster phase of phase 1
        > after beatig phase 1, the giant jumps at and throws rocks (or enemies?) at you

        */


        if(_attackCounter > 3){
            _attackCounter = 0;

            _groundPountTimer = 0;
            attackSide = Random.Range(0, 2);
            _attackArm = arms[attackSide];
        }

        _groundPountTimer += Time.deltaTime;

        if(_groundPountTimer < 1f){
            _attackArm.localPosition = Vector3.Lerp(_attackArm.localPosition, new Vector3(_attackArm.localPosition.x, 9, 0), Time.deltaTime * 3);
        }
        else if(_groundPountTimer < 1.3f){
            armHitBoxes[attackSide].SetActive(true);
            _attackArm.localPosition = Vector3.Lerp(_attackArm.localPosition, new Vector3(_attackArm.localPosition.x, -3, 0), Time.deltaTime * 20);
        }
        else{
            armHitBoxes[attackSide].SetActive(false);
            _attackArm.localPosition = Vector3.Lerp(_attackArm.localPosition, new Vector3(_attackArm.localPosition.x, 2.9f, 0), Time.deltaTime * 5);
        }
    }

    IEnumerator MoveTime(int direction){
        _moveDirection = direction;
        yield return new WaitForSeconds(2);
        _moveDirection = 0;
        yield return new WaitForSeconds(2);
        StartCoroutine(MoveTime(direction * -1));
    }
}
