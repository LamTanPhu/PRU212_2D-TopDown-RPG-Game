using System.Collections;
using UnityEngine;

public class EnemyGravityShooter : MonoBehaviour
{
    [SerializeField] private GameObject gravityBulletPrefab;
    [SerializeField] private float pushForce = 10f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            SpawnGravityBullet();
        }
    }

    private void SpawnGravityBullet()
    {
        Transform hole = FindClosestHole();
        if (hole == null) return;

        Vector2 spawnPosition = (player.position + hole.position) / 2;
        GameObject bullet = Instantiate(gravityBulletPrefab, spawnPosition, Quaternion.identity);

        GravityBullet gravityBullet = bullet.GetComponent<GravityBullet>();
        if (gravityBullet != null)
        {
            gravityBullet.SetTarget(hole.position, pushForce);
        }
    }

    private Transform FindClosestHole()
    {
        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        if (holes.Length == 0) return null;

        Transform closest = holes[0].transform;
        float minDistance = Vector2.Distance(transform.position, closest.position);

        foreach (GameObject hole in holes)
        {
            float distance = Vector2.Distance(transform.position, hole.transform.position);
            if (distance < minDistance)
            {
                closest = hole.transform;
                minDistance = distance;
            }
        }
        return closest;
    }
}
