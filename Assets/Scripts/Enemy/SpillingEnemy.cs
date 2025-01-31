using UnityEngine;
using Util;

public class SpillingEnemy : MonoBehaviour
{
    private Transform _target;
    private Timer _attackCounter;
    [SerializeField] private float attackRecovery;
    [SerializeField] private ParticleSystem spillAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _attackCounter = new Timer(attackRecovery);
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.position.x - _target.position.x) < 1 && !_attackCounter.IsRunning){
            _attackCounter.Restart();
            spillAttack.Play();
        }

        _attackCounter.Tick(Time.deltaTime);
    }
    
}
