using System;
using UnityEngine;

public class ParticleRecycler : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        ObjectPoolManager.RecycleObject(gameObject);
    }
}
