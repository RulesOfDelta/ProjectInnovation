using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room2 : MonoBehaviour
{
    [SerializeField] private float minSize = 15f;
    [SerializeField] private float maxSize = 50f;
    [SerializeField] private float wallHeight = 10f;

    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Transform floor;
    private List<Transform> walls;

    // Start is called before the first frame update
    void Start()
    {
        if(walls == null) walls = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Generate();
        }
    }

    [ContextMenu("Generate")]
    private void Generate()
    {
        Clear();

        var x = Random.Range(minSize, maxSize);
        var y = Random.Range(minSize, maxSize);
        // left wall
        var wall = Instantiate(wallPrefab, new Vector3(0, wallHeight / 2f, -y / 2f), 
            Quaternion.Euler(0, 0, 0), transform);
        wall.localScale = new Vector3(x, wallHeight, 1);
        walls.Add(wall);
        
        wall = Instantiate(wallPrefab, new Vector3(-x / 2f, wallHeight / 2f, 0),
            Quaternion.Euler(0, 90, 0), transform);
        wall.localScale = new Vector3(y, wallHeight, 1);
        walls.Add(wall);
        
        wall = Instantiate(wallPrefab, new Vector3(0, wallHeight / 2f, y / 2f),
            Quaternion.Euler(0, 180, 0), transform);
        wall.localScale = new Vector3(x, wallHeight, 1);
        walls.Add(wall);
        
        wall = Instantiate(wallPrefab, new Vector3(x / 2f, wallHeight / 2f, 0),
            Quaternion.Euler(0, 270, 0), transform);
        wall.localScale = new Vector3(y, wallHeight, 1);
        walls.Add(wall);
        
        // TODO no magic division by 10
        floor.localScale = new Vector3(x / 10, 1, y / 10);
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        if (walls == null) walls = new List<Transform>();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            foreach (var w in walls)
                Destroy(w.gameObject);
        }
        else
        {
            foreach (var w in walls)
                DestroyImmediate(w.gameObject);
        }
#else
        foreach (var w in walls)
                Destroy(w.gameObject);
#endif
        walls.Clear();
    }
}