using System;
using UnityEngine;

public class PlayerWallSound : MonoBehaviour
{
    [SerializeField] private LayerMask playerWallMask;
    [SerializeField] private LayerMask doorMask;
    private LayerMask useMask;
    [SerializeField] private float minVelocity = 1f;
    [FMODUnity.EventRef, SerializeField] private string wallHitEvent;
    [SerializeField] private float soundDist = 2.5f;
    [SerializeField] private InputHandler inputHandler;

    [FMODUnity.EventRef, SerializeField] private string hummingSound;
    private FMOD.Studio.EventInstance hummingInstance;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private AnimationCurve falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private bool bumped = false;
    public bool bumpedIntoWall => bumped;

    // Start is called before the first frame update
    void Start()
    {
        hummingInstance = hummingSound.CreateSound();
        hummingInstance.setParameterByName("HumVolume", 0f);
        hummingInstance.start();
        useMask = playerWallMask;
    }

    private void OnDestroy()
    {
        hummingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        hummingInstance.release();
    }

    // Update is called once per frame
    void Update()
    {
        float val;
        // TODO do raycasts to walls
        if (Physics.Raycast(transform.position, transform.forward,
            out var hit, maxDistance, useMask))
        {
            val = hit.distance < minDistance
                ? 1f
                : falloff.Evaluate((hit.distance - minDistance) / (maxDistance - minDistance));
        }
        else
            val = 0f;

        hummingInstance.setParameterByName("HumVolume", val);
        inputHandler.Rumble(val, val);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (useMask.Contains(other.gameObject.layer))
        {
            bumped = true;
            var point = other.GetContact(0).point;
            Debug.Log($"point {point} pos {transform.position}");
            point.y = transform.position.y;
            point += (point - transform.position).normalized * soundDist;
            Debug.Log($"sound pos {point}");
            FMODUnity.RuntimeManager.PlayOneShot(wallHitEvent, point);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        bumped = false;
    }

    public void OnClear()
    {
        useMask = playerWallMask;
        Debug.Log("Clear");
        MaskToNames(useMask);
    }

    public void OnGenerate()
    {
        useMask = playerWallMask | doorMask;
        Debug.Log("GENERATE");
        MaskToNames(useMask);
    }

    private static void MaskToNames(LayerMask original)
    {
        for (int i = 0; i < 32; ++i)
        {
            int shifted = 1 << i;
            if ((original & shifted) == shifted)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    Debug.Log(layerName);
                }
            }
        }
    }
}