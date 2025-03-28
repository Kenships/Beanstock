using System.Numerics;
using System.Threading;
using UnityEngine;

public class AOE : MonoBehaviour
{
    private float timer;


    private void Start()
    {

    }

    private void Update()
    {
        Timer();
    }

    private void Timer()
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            Destroy(gameObject);
        }
    }
   
}