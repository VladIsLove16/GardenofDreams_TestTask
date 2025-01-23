using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<Transform> SpawnPositions;
    [SerializeField] Enemy enemy;
    private void Awake()
    {
        foreach(Transform t in SpawnPositions)
        {
            Instantiate(enemy,t.position,Quaternion.identity);
        }
    }
}