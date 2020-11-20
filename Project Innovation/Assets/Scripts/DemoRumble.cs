using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRumble : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private InputHandler handler;

    // Update is called once per frame
    void Update()
    {
        var dist = Vector3.Distance(transform.position, target.position);
        var lowFreq = dist == 0f ? 0f : 1f / dist;
        var highFreq = dist == 0f ? 0f : lowFreq / dist;
        handler.Rumble(lowFreq, highFreq);
    }
}
