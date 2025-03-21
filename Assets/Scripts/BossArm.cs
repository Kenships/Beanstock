using UnityEngine;

public class BossArm : MonoBehaviour
{
    private Vector3 originalPosition;
    public GameObject attackHitBox;
    private BoxCollider2D hitBox;
    private float _slam;
    private float _clap;
    private float _attackCounter;
    [SerializeField] private SpriteRenderer mySprite;
    private Material normalMaterial;
    [SerializeField] private Material flash;
    [SerializeField] private AudioSource smash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalMaterial = mySprite.material;
        hitBox = gameObject.GetComponent<BoxCollider2D>();
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        _slam -= Time.deltaTime;
        _clap -= Time.deltaTime;

        if(_slam > 0){
            //ground slam attack
            if(_slam > 0.2f){
                //windup
                LerpMove(new Vector3(0, 5), 5);
            }
            else{
                //actual attack

                restartHitBox(0.4f);
                LerpMove(new Vector3(0, -7), 15);
                CameraScript.Shake(0.3f);
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
                CameraScript.Shake(0.3f);
            }
        }
        else{
            LerpMove(Vector3.zero, 8);
        }

        _attackCounter -= Time.deltaTime;

        if(_attackCounter > 0 && !attackHitBox.activeSelf)
        {
            hitBox.enabled = false;
            attackHitBox.SetActive(true);
            mySprite.material = flash;
            smash.Play();
        }
        else if(_attackCounter < 0 && attackHitBox.activeSelf)
        {
            mySprite.material = normalMaterial;
            hitBox.enabled = true;
            attackHitBox.SetActive(false);
        }

        
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
