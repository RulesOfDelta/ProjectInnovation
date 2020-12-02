using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Room2))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;

    //[SerializeField] [Tooltip("Only the xz-plane will be considered!")] private Vector3 spawnArea;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int minEnemiesSpawned = 1;
    [SerializeField] private int maxEnemiesSpawned = 25;

    private Room2 room;
    
    private  List<GameObject> enemies;

    public bool isIntroduction = true;

    private void Awake()
    {
        enemies = new List<GameObject>();
    }

    private void Start()
    {
        room = GetComponent<Room2>();
        //if(isIntroduction) SpawnEnemies(1, );
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

        for (int i = 0; i < amount; i++)
        {
            var enemyPrefab = enemyPrefabs.Random();
            Vector3 spawnPoint = new Vector3(Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                vectorToGround.y + enemyPrefab.transform.localScale.y,
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2));
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
}