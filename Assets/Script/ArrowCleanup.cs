using UnityEngine;

public class Arrow : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the arrow when it hits something (except the player)
        if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
