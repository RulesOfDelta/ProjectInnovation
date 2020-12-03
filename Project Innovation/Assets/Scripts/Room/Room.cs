using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    [Serializable]
    private struct Range
    {
        public float Min;
        public float Max;
    }

    [SerializeField] private Range sizeRange;
    [SerializeField] private float wallHeight = 10f;

    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private Transform topWall;
    [SerializeField] private Transform bottomWall;
    [SerializeField] private Transform floor;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Generate();
        }
    }

    [ContextMenu("Generate")]
    private void Generate()
    {
        var y = Random.Range(sizeRange.Min, sizeRange.Max);
        var x = Random.Range(sizeRange.Min, sizeRange.Max);
        rightWall.position = new Vector3(-x / 2f, 0f, 0f);
        rightWall.localScale = new Vector3(1f, wallHeight, y);
        leftWall.position = new Vector3(x / 2f, 0f, 0f);
        leftWall.localScale = new Vector3(1f, wallHeight, y);
        topWall.position = new Vector3(0f, 0f, -y / 2f);
        topWall.localScale = new Vector3(x, wallHeight, 1f);
        bottomWall.position = new Vector3(0, 0f, y / 2f);
        bottomWall.localScale = new Vector3(x, wallHeight, 1f);
        floor.localScale = new Vector3(x / 10, 1, y / 10);
    }
}