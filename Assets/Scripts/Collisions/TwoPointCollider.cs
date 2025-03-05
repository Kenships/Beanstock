using System;
using System.Collections.Generic;
using Events.Channels;
using UnityEngine;

namespace Collisions
{
    public class TwoPointCollider : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float width;
        [SerializeField] private float extraMargin;

        public GameObjectEventChannelSO onCollisionEnter;
        
        private void Awake()
        {
            if (onCollisionEnter == null)
            {
                onCollisionEnter = ScriptableObject.CreateInstance<GameObjectEventChannelSO>();
            }
        }
        
        public void Start()
        {
            float distance = Vector3.Distance(pointA.position, pointB.position) + extraMargin;
            transform.localScale = new Vector3(distance, width);
            
            Vector3 midPoint = (pointA.position + pointB.position) / 2;
            
            transform.position = midPoint;
            
            float distanceX = pointB.position.x - pointA.position.x;
            float distanceY = pointB.position.y - pointA.position.y;
            float angle = Mathf.Atan2(distanceY, distanceX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("ENTER");
            onCollisionEnter.RaiseEvent(other.gameObject);
        }
    }
}
