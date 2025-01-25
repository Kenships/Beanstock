using UnityEngine;

namespace Enemy
{
    public class WallLurker : MonoBehaviour
    {
        [SerializeField] private float detectionTime = 0.5f; // Time the player must stay to trigger the enemy
        [SerializeField] private float emergeSpeed = 10.0f; // Speed at which the enemy emerges from the wall
        [SerializeField] private float retreatSpeed = 1.0f; // Speed at which the enemy retreats into the wall
        [SerializeField] private float retreatDelay = 2.0f; // Delay before retreating after emerging

        private Collider2D ec;
        private bool isPlayerInRange = false;
        private float playerStayTimer = 0f;
        private float retreatTimer = 0f;
        private Vector3 hiddenPosition; // Position where the enemy is hidden
        private Vector3 emergedPosition; // Position where the enemy is fully emerged

        private enum EnemyState { Hidden, Emerging, Emerged, Retreating }
        private EnemyState currentState = EnemyState.Hidden;

        void Start()
        {
            ec = GetComponent<Collider2D>();
            // Set the hidden and emerged positions
            hiddenPosition = transform.position;
            emergedPosition = hiddenPosition + new Vector3(2f, 0, 0); // Adjust the forward direction and distance as needed
        }

        void Update()
        {
            switch (currentState)
            {
                case EnemyState.Hidden:
                    HandleHiddenState();
                    break;

                case EnemyState.Emerging:
                    HandleEmergingState();
                    break;

                case EnemyState.Emerged:
                    HandleEmergedState();
                    break;

                case EnemyState.Retreating:
                    HandleRetreatingState();
                    break;
            }
        }

        private void HandleHiddenState()
        {
            if (isPlayerInRange)
            {
                // Increment the timer while the player is in range
                playerStayTimer += Time.deltaTime;

                // Check if the player has stayed long enough
                if (playerStayTimer >= detectionTime)
                {
                    StartEmerging();
                }
            }
            else
            {
                // Reset the timer if the player leaves the range
                playerStayTimer = 0f;
            }
        }

        private void HandleEmergingState()
        {
            // Move the enemy from the hidden position to the emerged position
            transform.position = Vector3.MoveTowards(transform.position, emergedPosition, emergeSpeed * Time.deltaTime);

            // Stop emerging once the enemy reaches the emerged position
            if (transform.position == emergedPosition)
            {
                currentState = EnemyState.Emerged;
                retreatTimer = 0f; // Reset the retreat timer
            }
        }

        private void HandleEmergedState()
        {
            // Wait for the retreat delay before retreating
            retreatTimer += Time.deltaTime;
            if (retreatTimer >= retreatDelay)
            {
                StartRetreating();
            }
        }

        private void HandleRetreatingState()
        {
            // Move the enemy from the emerged position back to the hidden position
            transform.position = Vector3.MoveTowards(transform.position, hiddenPosition, retreatSpeed * Time.deltaTime);

            // Stop retreating once the enemy reaches the hidden position
            if (transform.position == hiddenPosition)
            {
                currentState = EnemyState.Hidden;
                ec.enabled = true; // Re-enable the collider
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = true;
                Debug.Log("Player entered the trigger zone!");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                playerStayTimer = 0f; // Reset the timer
                Debug.Log("Player exited the trigger zone!");
            }
        }

        private void StartEmerging()
        {
            currentState = EnemyState.Emerging;
            ec.enabled = false; // Disable the collider while emerging
        }

        private void StartRetreating()
        {
            currentState = EnemyState.Retreating;
        }
    }
}