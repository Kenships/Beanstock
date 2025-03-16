using System.Collections.Generic;
using Events.Channels;
using UnityEngine;

namespace UI
{
    public class UIHeartHolder : MonoBehaviour
    {
        [SerializeField] private FloatEventChannelSO onHealthChange;
        [SerializeField] private GameObject heartPrefab;
        private List<GameObject> _hearts;

        private void Awake()
        {
            _hearts = new List<GameObject>();
        }
        private void Start()
        {
            onHealthChange.onEventRaised += SetHearts;
        }

        private void SetHearts(float HP)
        {
            for (int i = _hearts.Count - 1; i >= 0; i--)
            {
                _hearts.Remove(ObjectPoolManager.RecycleObject(_hearts[i]));
            }

            for (int i = 0; i < (int)HP; i++)
            {
                _hearts.Add(ObjectPoolManager.SpawnObject(heartPrefab, transform));
            }
        }
    }
}
