using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSound : MonoBehaviour
{
    [SerializeField] private LayerMask playerWallMask;
    [SerializeField] private float minVelocity = 1f;
    [FMODUnity.EventRef, SerializeField] private string wallHitEvent;
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private AnimationCurve falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO do raycasts to walls
        if (Physics.Raycast(transform.position, transform.forward, 
            out var hit, maxDistance, playerWallMask))
        {
            var val = hit.distance < minDistance
                ? 1f 
                : falloff.Evaluate((hit.distance - minDistance) / (maxDistance - maxDistance));
            inputHandler.Rumble(val, val);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Debug.Log("Collision");
        // if (playerWallMask.Contains(other.gameObject.layer) && 
        //     other.relativeVelocity.magnitude > minVelocity)
        // {
        //     Debug.Log("Play Sound");
        //     FMODUnity.RuntimeManager.PlayOneShot(wallHitEvent, other.GetContact(0).point);
        // }
    }
}
