using UnityEngine;

namespace Enemy
{
    public class WallLurker : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private float detectionTime = 0.5f; // Time the player must stay to trigger the enemy
        [SerializeField] private float emergeSpeed = 10.0f; // Speed at which the enemy emerges from the wall
        [SerializeField] private float retreatSpeed = 1.0f; // Speed at which the enemy retreats into the wall
        [SerializeField] private float retreatDelay = 2.0f; // Delay before retreating after emerging

        [Header("Leaf Effect Settings")]
        [SerializeField] private ParticleSystem leafEffect; // Reference to the leaf particle system
        [SerializeField] private float maxEmissionRate = 50f; // Maximum emission rate when player stays for full detection time

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

            // Ensure the leaf effect is initially stopped
            if (leafEffect != null)
            {
                leafEffect.Stop();
            }
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

                // Update the leaf effect intensity
                UpdateLeafEffect();

                // Check if the player has stayed long enough
                if (playerStayTimer >= detectionTime)
                {
                    StartEmerging();
                }
            }
            else
            {
                // Reset the timer and stop the leaf effect if the player leaves the range
                playerStayTimer = 0f;
                if (leafEffect != null)
                {
                    leafEffect.Stop();
                }
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

                // Start the leaf effect when the player enters
                if (leafEffect != null)
                {
                    leafEffect.Play();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                playerStayTimer = 0f; // Reset the timer
                Debug.Log("Player exited the trigger zone!");

                // Stop the leaf effect when the player exits
                if (leafEffect != null)
                {
                    leafEffect.Stop();
                }
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

        private void UpdateLeafEffect()
        {
            if (leafEffect != null)
            {
                // Calculate the emission rate based on the playerStayTimer
                float emissionRate = Mathf.Lerp(0, maxEmissionRate, playerStayTimer / detectionTime);

                // Update the particle system's emission rate
                var emission = leafEffect.emission;
                emission.rateOverTime = emissionRate;
            }
        }
    }
}