using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad; // Set this in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision Detected with: " + other.gameObject.name); // Debug message

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the exit trigger. Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
