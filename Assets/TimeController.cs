using UnityEngine;

public class TimeController : MonoBehaviour
{
    private const float timeReturnSpeed = 30;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * timeReturnSpeed);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public static void setTime(float amount){
        Time.timeScale = amount;
    }
}
