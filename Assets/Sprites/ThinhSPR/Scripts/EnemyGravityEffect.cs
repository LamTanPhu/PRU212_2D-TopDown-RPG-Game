using UnityEngine;

public class EnemyGravityEffect : MonoBehaviour
{
    [SerializeField] private float pullForce = 10f; // Pull player towards hole
    [SerializeField] private float pushForce = 20f; // Push player into hole
    [SerializeField] private float pushDistance = 2f; // Distance threshold to push player
    private Transform player;
    private Rigidbody2D playerRb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                ApplyGravityEffect();
            }
        }
    }

    private void ApplyGravityEffect()
    {
        Transform hole = FindClosestHole();
        if (hole != null)
        {
            Vector2 pullDirection = (hole.position - player.position).normalized;

            // Pull the player towards the hole
            playerRb.linearVelocity = pullDirection * pullForce;

            // If close enough, apply a push force to ensure the player falls into the hole
            if (Vector2.Distance(player.position, hole.position) < pushDistance)
            {
                playerRb.AddForce(pullDirection * pushForce, ForceMode2D.Impulse);
            }
        }
    }

    private Transform FindClosestHole()
    {
        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        Transform closestHole = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject hole in holes)
        {
            float distance = Vector2.Distance(player.position, hole.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHole = hole.transform;
            }
        }

        return closestHole;
    }
}
