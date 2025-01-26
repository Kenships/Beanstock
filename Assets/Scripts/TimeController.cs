using System.Collections;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private const float returnRate = 40;

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * returnRate);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public static void setTime(float t){
        Time.timeScale = t;
    }   

    public static IEnumerator freezeTime(float t){
        Debug.Log("freeze!");
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(t);
        Time.timeScale = 1;
    }
}
