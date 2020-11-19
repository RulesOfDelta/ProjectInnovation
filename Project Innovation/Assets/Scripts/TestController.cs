using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private InputHandler handler;

    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float moveSpeed = 10f;

    private void Start()
    {
        handler.RegisterOnFire(OnFire);
    }

    private void OnDestroy()
    {
        handler.DeregisterOnFire(OnFire);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * handler.Look.x * rotSpeed, 0);
        var move = handler.Move * (Time.deltaTime * moveSpeed);
        transform.Translate(move.x, 0, move.y);
    }

    private void OnFire()
    {
        transform.Translate(transform.forward * 10f);
    }
}
