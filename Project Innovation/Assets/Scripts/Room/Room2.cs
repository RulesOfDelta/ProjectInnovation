﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Room2 : MonoBehaviour
{
    [SerializeField] private float minSize = 15f;
    [SerializeField] private float maxSize = 50f;
    [SerializeField] private float wallHeight = 10f;

    [SerializeField] private float doorWidth = 2f;
    [SerializeField, Range(0f, 1f)] private float doorSpace = 0.5f;

    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Transform floor;
    private List<Transform> walls;

    [SerializeField, Min(0)] private int minDoorCount;
    [SerializeField, Min(0)] private int maxDoorCount;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Generate();
        }
    }

    private readonly struct Wall
    {
        // TODO naming
        public readonly float Distance;
        public readonly float AxisPos;
        public readonly Quaternion Rotation;
        public readonly float Size;
        public readonly bool Horizontal;

        private Wall(float distance, float axisPos, Quaternion rotation, float size, bool horizontal)
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

    [ContextMenu("Generate")]
    private void EditModeGenerate()
    {
        if (!inEditMode)
        {
            EditorApplication.playModeStateChanged += EditModeHandleStateChange;
            inEditMode = true;
        }
        Generate();
    }

    private void EditModeHandleStateChange(PlayModeStateChange newState)
    {
        if (newState != PlayModeStateChange.ExitingEditMode) return;
        EditorApplication.playModeStateChanged -= EditModeHandleStateChange;
        Clear();
        inEditMode = false;
    }

    private void Generate()
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

        var splitCount = Random.Range(minDoorCount, maxDoorCount);
        for (var i = 0; i < splitCount; i++) SplitWallInList(preWalls, Random.Range(0, preWalls.Count - 1));

        FillFromWalls(preWalls);
        // TODO no magic division by 10
        floor.localScale = new Vector3(x / 10, 1, y / 10);
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
        SplitWall(wall).Deconstruct(out var wall1, out var wall2);
        wallList.Add(wall1);
        wallList.Add(wall2);
    }

    // TODO also return door position (where the split was)
    private Tuple<Wall, Wall> SplitWall(Wall wall)
    {
        // TODO parameters for door width and empty room at each side
        var maxDist = (wall.Size / 2f - doorWidth) * doorSpace;
        var split = Random.Range(-maxDist, maxDist);
        // TODO leave room for door
        var sizeA = wall.Size / 2f - split - doorWidth / 2f;
        sizeA = Mathf.Max(0f, sizeA);
        var sizeB = wall.Size / 2f + split - doorWidth / 2f;
        sizeB = Mathf.Max(0f, sizeB);
        var splitPosA = wall.AxisPos - split - sizeA / 2f - doorWidth / 2f;
        var splitPosB = wall.AxisPos - split + sizeB / 2f + doorWidth / 2f;

        return new Tuple<Wall, Wall>(wall.Split(splitPosA, sizeA),
            wall.Split(splitPosB, sizeB));
    }

    // TODO HORRIBLE NAMING
    private void FillFromWalls(List<Wall> wallStructs)
    {
        foreach (var wall in wallStructs)
        {
            var w = Instantiate(wallPrefab, PosFromWall(wall), wall.Rotation, transform);
            w.localScale = new Vector3(wall.Size, wallHeight, 1);
            walls.Add(w);
        }

        Vector3 PosFromWall(Wall w)
        {
            return w.Horizontal
                ? new Vector3(w.Distance, wallHeight / 2f, w.AxisPos)
                : new Vector3(w.AxisPos, wallHeight / 2f, w.Distance);
        }
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        if (walls == null) walls = new List<Transform>();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            foreach (var w in walls)
            {
                if(w)
                    Destroy(w.gameObject);
            }
        }
        else
        {
            foreach (var w in walls)
            {
                if(w)
                    DestroyImmediate(w.gameObject);
            }
        }
#else
        foreach (var w in walls) 
        {
            if(w)
                Destroy(w.gameObject);
        }
#endif
        walls.Clear();
    }
}