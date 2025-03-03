using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class HoleSceneTransition : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "NextScene"; // Name of the scene to transition to (set in Inspector)
    [SerializeField] private float transitionDelay = 0.5f;         // Delay before transitioning (optional)
    [SerializeField] private string spawnPointName = "SpawnPoint"; // Name of the spawn point object in the target scene

    private bool isTransitioning = false;                         // Prevent multiple transitions

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collides with an object tagged "Hole" and isn't already transitioning
        if (other.CompareTag("Hole") && !isTransitioning)
        {
            Debug.Log("Player entered a hole! Transitioning to " + targetSceneName);

            // Mark as transitioning to prevent multiple triggers
            isTransitioning = true;

            // Optionally, disable player movement or add visual effects here
            StartCoroutine(TransitionToScene());
        }
    }

    private System.Collections.IEnumerator TransitionToScene()
    {
        // Optional: Wait for a delay before transitioning
        if (transitionDelay > 0f)
        {
            yield return new WaitForSeconds(transitionDelay);
        }

        // Preserve the player across scenes
        DontDestroyOnLoad(gameObject);

        // Load the target scene
        SceneManager.LoadScene(targetSceneName);

        // Wait until the new scene is fully loaded
        yield return new WaitForSeconds(0.1f); // Small delay to ensure scene is loaded

        // Find the spawn point by name in the target scene
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null)
        {
            // Reposition the player at the spawn point
            transform.position = spawnPoint.transform.position;
            Debug.Log("Player spawned at: " + spawnPoint.transform.position);
        }
        else
        {
            Debug.LogError("SpawnPoint named '" + spawnPointName + "' not found in " + targetSceneName + "! Using current position.");
        }

        // Allow transitions again after positioning
        isTransitioning = false;
    }

    // Optional: Method to reset or handle player state before transition
    private void OnDisable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Stop player movement during transition
        }
    }
}