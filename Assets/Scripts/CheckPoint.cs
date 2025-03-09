using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private int checkPointIndex;
        public int CheckPointIndex => checkPointIndex;

        private void Start()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

}