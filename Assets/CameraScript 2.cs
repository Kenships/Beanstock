using Unity.Cinemachine;
using UnityEngine;

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
        shake.AmplitudeGain = shakeAmount;
    }

    public static void Shake(float amount){
        if(amount > shakeAmount)
            shakeAmount = amount;
    }
}
