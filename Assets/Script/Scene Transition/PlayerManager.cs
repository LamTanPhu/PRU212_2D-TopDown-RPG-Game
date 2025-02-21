using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab; // Assign the Player prefab in the Inspector

    private void Start()
    {
        // Get the saved spawn point name from PlayerPrefs
        string spawnPointName = PlayerPrefs.GetString("SpawnPoint", "DefaultSpawn");
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SpawnPoint not found: " + spawnPointName);
        }
    }
}
