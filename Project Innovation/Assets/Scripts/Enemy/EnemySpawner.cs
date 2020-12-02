using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Room2))]
public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawn
    {
        public GameObject prefab;
        [Min(0f)] public float percentage;
    }
    
    [SerializeField] private List<EnemySpawn> enemyPrefabs;

    [SerializeField] private int minEnemiesSpawned = 1;
    [SerializeField] private int maxEnemiesSpawned = 25;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private int spawnTries = 10;

    private Room2 room;
    
    private  List<GameObject> enemies;

    private void Awake()
    {
        enemies = new List<GameObject>();
    }

    private void Start()
    {
        room = GetComponent<Room2>();
        ValidatePercentages();
    }

    // private void SpawnEnemyIntervals()
    // {
    // }

    public void SpawnEnemies(int amount, Vector2 spawnArea)
    {
        amount = Mathf.Clamp(amount, minEnemiesSpawned, maxEnemiesSpawned);
        ClearEnemies();
        Vector3 vectorToGround;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit,
            float.PositiveInfinity))
        {
            vectorToGround = raycastHit.point;
        }
        else vectorToGround = transform.position;

        for (var i = 0; i < amount; i++)
        {
            var enemyPrefab = GetRandomEnemy();
            if(enemyPrefab == null) continue;
            var spawnPoint = new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                vectorToGround.y + enemyPrefab.transform.localScale.y,
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2));
            for (var j = 0; j < spawnTries && !ValidPos(spawnPoint); j++)
            {
                spawnPoint = new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                    vectorToGround.y + enemyPrefab.transform.localScale.y,
                    Random.Range(-spawnArea.y / 2, spawnArea.y / 2));
            }
            var enemy = Instantiate(enemyPrefab, spawnPoint,
                Quaternion.identity * Quaternion.Euler(0, Random.rotation.eulerAngles.y, 0));
            enemy.GetComponent<EnemyRemoveNotifier>().AddEvent(OnRemoveEnemy);
            enemies.Add(enemy);
        }
    }

    private bool clearing;

    private void OnRemoveEnemy(GameObject enemy)
    {
        if (clearing) return;
        enemies.Remove(enemy);
        if (!HasEnemies()) 
            room.OnClear();
        // TODO notify room
    }

    private void ClearEnemies()
    {
        clearing = true;
        foreach (var enemy in enemies)
        {
            if (enemy)
                Destroy(enemy);
        }
        
        enemies.Clear();
        clearing = false;
    }

    public bool HasEnemies()
    {
        return !enemies.TrueForAll(obj => !obj);
    }

    // public void SetSpawnArea(Vector3 area)
    // {
    //     spawnArea = new Vector3(area.x, 0, area.z);
    // }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }


    private void ValidatePercentages()
    {
        if (enemyPrefabs == null) return;
        var total = enemyPrefabs.Sum(prefab => prefab.percentage);
        if (total == 0f)
        {
            foreach (var enemySpawn in enemyPrefabs)
            {
                enemySpawn.percentage = 1f / enemyPrefabs.Count;
            }
        }
        else
        {
            total = Mathf.Abs(total);
            foreach (var enemySpawn in enemyPrefabs)
            {
                enemySpawn.percentage /= total;
            }
        }
    }

    private GameObject GetRandomEnemy()
    {
        var rand = Random.value;
        var total = 0f;
        foreach (var enemySpawn in enemyPrefabs)
        {
            total += enemySpawn.percentage;
            if (rand < total)
                return enemySpawn.prefab;
        }

        return null;
    }

    private bool ValidPos(Vector3 pos)
    {
        return enemies.All(enemy => Vector3.Distance(enemy.transform.position, pos) > minDistance);
    }
}