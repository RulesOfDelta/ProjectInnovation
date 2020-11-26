﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemySpawner))]
public class Room2 : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float minSize = 15f;
    [SerializeField] private float maxSize = 50f;
    [SerializeField] private float wallHeight = 10f;

    [SerializeField] private float doorWidth = 2f;

    [SerializeField, Range(0f, 1f)] private float doorSpace = 0.5f;

    // TODO use something like a door class to insert reference to this object
    [SerializeField] private Door doorPrefab;

    [SerializeField] private float entryWidth = 4f;
    [SerializeField] private float entryDepth = 4f;

    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Transform floor;
    private List<Transform> walls;
    private List<Door> doors;

    [SerializeField, Min(0)] private int minDoorCount;
    [SerializeField, Min(0)] private int maxDoorCount;
    
    [SerializeField, Range(0, 1f)] private float enemySpawnPercentageX;
    [SerializeField, Range(0, 1f)] private float enemySpawnPercentageY;
    [SerializeField, Min(0f)] private float enemySpawnPerSqrUnit;
    private EnemySpawner enemySpawner;

    // Start is called before the first frame update
    private void Start()
    {
        if (!enemySpawner) enemySpawner = GetComponent<EnemySpawner>();
        Generate();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Generate();
        }
    }

    private void OnValidate()
    {
        SetupEditMode();
    }

    private struct Wall
    {
        // TODO naming
        public float Distance;
        public float AxisPos;
        public Quaternion Rotation;
        public float Size;
        public bool Horizontal;

        public Wall(float distance, float axisPos, Quaternion rotation, float size, bool horizontal)
        {
            Distance = distance;
            AxisPos = axisPos;
            Rotation = rotation;
            Size = size;
            Horizontal = horizontal;
        }

        public Wall(float distance, Quaternion rotation, float size, bool horizontal)
        {
            Distance = distance;
            AxisPos = 0f;
            Rotation = rotation;
            Size = size;
            Horizontal = horizontal;
        }

        public Wall Split(float splitPos, float size)
        {
            return new Wall(Distance, splitPos, Rotation, size, Horizontal);
        }
    }

    private bool inEditMode;

    public void EditModeGenerate()
    {
        SetupEditMode();

        Generate();
    }

    private void SetupEditMode()
    {
#if UNITY_EDITOR
        if (inEditMode) return;
        UnityEditor.EditorApplication.playModeStateChanged += EditModeHandleStateChange;
        inEditMode = true;
#endif
    }

#if UNITY_EDITOR
    private void EditModeHandleStateChange(UnityEditor.PlayModeStateChange newState)
    {
        if (newState != UnityEditor.PlayModeStateChange.ExitingEditMode) return;
        UnityEditor.EditorApplication.playModeStateChanged -= EditModeHandleStateChange;
        Clear();
        inEditMode = false;
    }
#endif
    
    public void Generate()
    {
        Clear();

        var x = Random.Range(minSize, maxSize);
        var y = Random.Range(minSize, maxSize);

        var preWalls = new List<Wall>
        {
            new Wall(-y / 2f, Quaternion.Euler(0, 0, 0), x, false),
            new Wall(-x / 2f, Quaternion.Euler(0, 90, 0), y, true),
            new Wall(y / 2f, Quaternion.Euler(0, 180, 0), x, false),
            new Wall(x / 2f, Quaternion.Euler(0, 270, 0), y, true)
        };

        var splitCount = Mathf.Min(Random.Range(minDoorCount, maxDoorCount + 1), preWalls.Count);
        var tempWalls = new List<Wall>();
        for (var i = 0; i < splitCount; i++)
        {
            var num = Random.Range(0, preWalls.Count - 1);
            tempWalls.Add(preWalls[num]);
            preWalls.RemoveAt(num);
        }

        var entryWallIndex = Random.Range(0, preWalls.Count);
        var entryWall = preWalls[entryWallIndex];
        preWalls.RemoveAt(entryWallIndex);
        // TODO create entry
        AddEntry(preWalls, entryWall);

        foreach (var t in tempWalls)
        {
            SplitWallAndAdd(preWalls, t);
        }

        tempWalls.Clear();

        FillFromWalls(preWalls);
        // TODO no magic division by 10
        floor.localScale = new Vector3((x + entryDepth * 2) / 10, 1, (y + entryDepth * 2) / 10);
        
        // TODO spawn enemies
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        var spawnX = x * enemySpawnPercentageX;
        var spawnY = y * enemySpawnPercentageY;
        var enemyCount = Mathf.CeilToInt(spawnX * spawnY * enemySpawnPerSqrUnit);
        enemySpawner.SpawnEnemies(enemyCount, new Vector2(spawnX, spawnY));
    }

    private void SplitWallAtEnd(IList<Wall> wallList)
    {
        SplitWallInList(wallList, wallList.Count - 1);
    }

    private void SplitWallInList(IList<Wall> wallList, int index)
    {
        var wall = wallList[index];
        wallList.RemoveAt(index);
        SplitWallAndAdd(wallList, wall);
    }

    private void SplitWallAndAdd(ICollection<Wall> wallList, Wall wall)
    {
        SplitWall(wall, true).Deconstruct(out var wall1, out var wall2, out _);
        wallList.Add(wall1);
        wallList.Add(wall2);
    }

    private Tuple<Wall, Wall, float> SplitWall(Wall wall, bool makeDoor = false)
    {
        return SplitWall(wall, doorWidth, makeDoor);
    }

    // TODO also return door position (where the split was)
    private Tuple<Wall, Wall, float> SplitWall(Wall wall, float gapWidth, bool makeDoor = false)
    {
        // TODO parameters for door width and empty room at each side
        var maxDist = (wall.Size / 2f - gapWidth) * doorSpace;
        var split = Random.Range(-maxDist, maxDist);
        // TODO leave room for door
        var sizeA = wall.Size / 2f - split - gapWidth / 2f;
        sizeA = Mathf.Max(0f, sizeA);
        var sizeB = wall.Size / 2f + split - gapWidth / 2f;
        sizeB = Mathf.Max(0f, sizeB);
        var splitPosA = wall.AxisPos - split - sizeA / 2f - gapWidth / 2f;
        var splitPosB = wall.AxisPos - split + sizeB / 2f + gapWidth / 2f;

        if (makeDoor)
        {
            // Create a door
            // TODO simpler method for posfrom wall
            var door = Instantiate(doorPrefab,
                PosFromWeird(wall.Distance, wall.AxisPos - split, wall.Horizontal), wall.Rotation,
                transform);
            door.transform.localScale = new Vector3(gapWidth, wallHeight, 1f);
            door.InsertRoom(this);
            doors.Add(door);
        }

        return new Tuple<Wall, Wall, float>(wall.Split(splitPosA, sizeA),
            wall.Split(splitPosB, sizeB), wall.AxisPos - split);
    }

    private void AddEntry(ICollection<Wall> wallList, Wall wall)
    {
        SplitWall(wall, entryWidth).Deconstruct(out var wall1, out var wall2, out var splitPos);
        wallList.Add(wall1);
        wallList.Add(wall2);
        var dist = wall.Distance < 0f ? -entryDepth : entryDepth;
        wallList.Add(new Wall(wall.Distance + dist, splitPos, wall.Rotation, entryWidth, wall.Horizontal));

        // Side walls
        wallList.Add(new Wall(splitPos + entryWidth / 2f, wall.Distance + dist / 2f,
            Quaternion.Euler(0, wall.Rotation.eulerAngles.y + 90f, 0), entryDepth, !wall.Horizontal));
        wallList.Add(new Wall(splitPos - entryWidth / 2f, wall.Distance + dist / 2f,
            Quaternion.Euler(0, wall.Rotation.eulerAngles.y - 90f, 0), entryDepth, !wall.Horizontal));

        var posWall = wall;
        posWall.AxisPos = splitPos;
        posWall.Distance += dist / 2f;

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

        var controller = player.GetComponent<SimpleCharacterController>();
        controller.ResetController();
        controller.EnableControls(false);

        var playerPos = transform.position + PosFromWall(posWall);
        playerPos.y = player.position.y;
        player.position = playerPos;
        var sign = Mathf.Sign(posWall.Distance);
        player.forward = posWall.Horizontal ? new Vector3(-sign, 0, 0) : new Vector3(0, 0, -sign);
        controller.EnableControls(true);
    }

    // TODO HORRIBLE NAMING
    private void FillFromWalls(IEnumerable<Wall> wallStructs)
    {
        foreach (var wall in wallStructs)
        {
            var w = Instantiate(wallPrefab, PosFromWall(wall), wall.Rotation, transform);
            w.localScale = new Vector3(wall.Size, wallHeight, 1);
            walls.Add(w);
        }
    }

    private Vector3 PosFromWall(Wall w)
    {
        return PosFromWeird(w.Distance, w.AxisPos, w.Horizontal);
    }

    private Vector3 PosFromWeird(float distance, float axisPos, bool horizontal)
    {
        return horizontal
            ? new Vector3(distance, wallHeight / 2f, axisPos)
            : new Vector3(axisPos, wallHeight / 2f, distance);
    }

    public void Clear()
    {
        if (walls == null) walls = new List<Transform>();
        if (doors == null) doors = new List<Door>();
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            ClearWithFunc(Destroy, doors);
            ClearWithFunc(Destroy, walls);
        }
        else
        {
            ClearWithFunc(DestroyImmediate, doors);
            ClearWithFunc(DestroyImmediate, walls);
        }
#else
        ClearWithFunc(Destroy, doors);
        ClearWithFunc(Destroy, walls);
#endif
        void ClearWithFunc<T>(Action<GameObject> destroyFunc, List<T> toClear) where T : Component
        {
            toClear.ForEach(t =>
            {
                if (t) destroyFunc(t.gameObject);
            });
            toClear.Clear();
        }
    }

    public void OnClear()
    {
        foreach (var door in doors)
        {
            if(door) door.OnAllEnemiesClear();
        }
    }
}