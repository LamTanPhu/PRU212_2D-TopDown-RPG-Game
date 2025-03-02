using UnityEngine;
using System.Collections;

public class EnemyAIMid : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Chasing
    }

    [SerializeField] private float detectionRange = 5f; // Range to detect player
    [SerializeField] private float roamRadius = 3f; // Random movement range
    [SerializeField] private float minX, maxX, minY, maxY; // Map boundaries

    private State state;
    private EnemyPathfindingMid enemyPathfinding;
    private Transform player;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfindingMid>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(AIBehaviorRoutine());
    }

    private IEnumerator AIBehaviorRoutine()
    {
        while (true)
        {
            if (player != null && Vector2.Distance(transform.position, player.position) < detectionRange)
            {
                state = State.Chasing;
            }
            else
            {
                state = State.Roaming;
            }

            if (state == State.Roaming)
            {
                Vector2 roamPosition = GetRoamingPosition();
                enemyPathfinding.MoveTo(roamPosition);
            }
            else if (state == State.Chasing)
            {
                enemyPathfinding.MoveTo(player.position);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }
}
