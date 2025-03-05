using UnityEngine;

public class BossArm : MonoBehaviour
{
    private Vector3 originalPosition;
    public GameObject attackHitBox;
    private float _slam;
    private float _clap;
    private float _attackCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        _slam -= Time.deltaTime;
        _clap -= Time.deltaTime;

        if(_slam > 0){
            //ground slam attack
            if(_slam > 0.3f){
                //windup
                LerpMove(new Vector3(0, 5), 5);
            }
            else{
                //actual attack
                restartHitBox(0.3f);
                LerpMove(new Vector3(0, -7), 15);
            }
        }
        else if(_clap > 0){
            //double arm clap
            if(_clap > 0.3f){
                //windup
                LerpMove(new Vector3(armSide() * 3, 0), 5);
            }
            else{
                //actual attack
                restartHitBox(0.3f);
                LerpMove(new Vector3(armSide() * -5.5f, 0), 13);
            }
        }
        else{
            LerpMove(Vector3.zero, 8);
        }

        _attackCounter -= Time.deltaTime;

        if(_attackCounter > 0 && !attackHitBox.activeSelf)
            attackHitBox.SetActive(true);
        else if(_attackCounter < 0 && attackHitBox.activeSelf)
            attackHitBox.SetActive(false);

        
    }

    private void restartHitBox(float set){
        if(_attackCounter < 0){
            _attackCounter = set;
        }
    }

    private float armSide(){
        return originalPosition.x / Mathf.Abs(originalPosition.x);
    }

    private void LerpMove(Vector3 aim, float speed)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + aim, Time.deltaTime * speed);
    }

    public void startSlam(){
        _slam = 1;
    }

    public void startClap(){
        _clap = 1.3f;
    }
}
