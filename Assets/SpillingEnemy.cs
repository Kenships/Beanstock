using UnityEngine;

public class SpillingEnemy : MonoBehaviour
{
    [SerializeField] private ParticleSystem Attack;
    [SerializeField] private float attackLength;
    [SerializeField] private float attackDelay;
    private enemyGroundLogic _enemyLogic;
    private float _attackCounter;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyLogic = gameObject.GetComponent<enemyGroundLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.position.x - _enemyLogic._chaseTarget.position.x) < 2 && _attackCounter < 1){
            _attackCounter = attackLength;
        }

        if(_attackCounter > 0 && _attackCounter < attackLength - attackDelay){
            if(!Attack.isPlaying){
                Attack.Play();
            }
        }
        else{
            if(Attack.isPlaying){
                Attack.Stop();
            }
        }

        _attackCounter -= Time.deltaTime;
    }
}
