using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Player transform
    public float smoothSpeed = 5f;

    void Start()
    {

        FindPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // Try to find the player again if the target is lost
            FindPlayer();
        }
        else
        {
            Vector3 desiredPosition = target.position + new Vector3(0f, 0f, -10f); 
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }
}
