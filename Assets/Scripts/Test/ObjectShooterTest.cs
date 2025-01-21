
using UnityEngine;

namespace Test
{
    public class SquareShooterTest : MonoBehaviour
    {
        public GameObject squarePrefab; // Prefab for the squares
        public float spawnRate = 0.2f; // How often to spawn squares (in seconds while holding the button)
        public float forceAmount = 5f; // Force applied to squares

        private float spawnTimer = 0f;

        private void Update()
        {
            // Check if the left mouse button is held down
            if (Input.GetMouseButton(0)) // 0 = Left Mouse Button
            {
                spawnTimer += Time.deltaTime;

                // Spawn squares at the specified spawn rate
                if (spawnTimer >= spawnRate)
                {
                    SpawnSquare();
                    spawnTimer = 0f;
                }
            }
            else
            {
                spawnTimer = 0f; // Reset the timer when the button is not held
            }
        }

        private void SpawnSquare()
        {
            // Instantiate the square at the position of this GameObject
            GameObject square = ObjectPoolManager.SpawnObject(squarePrefab, transform.position, Quaternion.identity, PoolType.GameObject);

            // Get the Rigidbody2D of the square and apply a random force
            Rigidbody2D rb = square.GetComponent<Rigidbody2D>();

            // Generate a random 2D direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Apply force in the random direction
            rb.AddForce(randomDirection * forceAmount, ForceMode2D.Impulse);

            // Start the coroutine to destroy the square after 5 seconds
            StartCoroutine(DestroyAfterDelay(square, 3f));
        }

        private System.Collections.IEnumerator DestroyAfterDelay(GameObject square, float delay)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay
            ObjectPoolManager.RecycleObject(square); // Destroy the square
        }
    }
}
