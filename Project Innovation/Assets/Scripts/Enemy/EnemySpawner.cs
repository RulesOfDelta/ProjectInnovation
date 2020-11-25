using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    //[SerializeField] [Tooltip("Only the xz-plane will be considered!")] private Vector3 spawnArea;
    [SerializeField] private float spawnInterval;
    
    // private void SpawnEnemyIntervals()
    // {
    // }

    public void SpawnEnemies(int amount, Vector2 spawnArea)
    {
        Vector3 vectorToGround;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, float.PositiveInfinity))
        {
            vectorToGround = raycastHit.point;
        }
        else vectorToGround = transform.position;
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2), 
                vectorToGround.y + enemy.transform.localScale.y,Random.Range(-spawnArea.y / 2, spawnArea.y / 2));
            Instantiate(enemy, spawnPoint, Quaternion.identity * Quaternion.Euler(0,Random.rotation.eulerAngles.y,0));
        }
    }

    // public void SetSpawnArea(Vector3 area)
    // {
    //     spawnArea = new Vector3(area.x, 0, area.z);
    // }
}