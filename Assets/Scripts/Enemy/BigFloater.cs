using UnityEngine;

namespace Enemy
{
    public class BigFloater: AbstractEnemy
    {
        private void Update()
        {
            MoveTowards(_originalPosition);
        }

        private void MoveTowards(Vector3 position){
            transform.up = new Vector3(transform.position.x - position.x, transform.position.y - position.y);
            _rb.linearVelocity += (Vector2) transform.up * (moveSpeed * Time.deltaTime);
            transform.up = Vector2.zero;
        }
    }
}