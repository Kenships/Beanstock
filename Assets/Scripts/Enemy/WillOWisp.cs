using UnityEngine;

namespace Enemy
{
    public class WillOWisp : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float wispSpeed;
        [SerializeField] private float portalDuration;
        [SerializeField] private Transform portalTarget;

        private bool _isTowardsB = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetPosition = _isTowardsB ? pointB.position : pointA.position;
        
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, wispSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                _isTowardsB = !_isTowardsB;
            }
        }
    }
}
