using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin shake;
    public static float shakeAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shakeAmount = Mathf.Lerp(shakeAmount, 0, Time.deltaTime * 5);
        if(shake != null)
            shake.AmplitudeGain = shakeAmount;
    }

    public static void Shake(float amount){
        if(amount > shakeAmount)
            shakeAmount = amount;
    }

    public void StartGame(){
        SceneManager.LoadScene("PlayerTest");
    }
}
