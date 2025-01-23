using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<Transform> SpawnPositions;
    [SerializeField] Enemy enemy;
    [SerializeField] int EnemyCount = 3;
    private void Awake()
    {
        for (int i = 0; i < EnemyCount; i++)
        {
            int j = Random.Range(0, SpawnPositions.Count);
            Instantiate(enemy, SpawnPositions[j].position,Quaternion.identity);
        }
    }
}