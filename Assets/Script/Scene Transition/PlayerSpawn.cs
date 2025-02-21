using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        string spawnPointName = PlayerPrefs.GetString("SpawnPoint", "DefaultSpawn"); // Default if not set
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("SpawnPoint not found: " + spawnPointName);
        }
    }
}
