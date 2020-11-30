﻿using UnityEngine;
using UnityEngine.UIElements;

public class WalkingSound : MonoBehaviour
{
    private float _distanceTraveled;
    private Vector3 _oldPosition;
    public float stepDistance = 1.2f;
    private float _randomStepSDistance;
    protected GameObject[] puddles;

    [FMODUnity.EventRef]
    public string fmodEventPath;

    void Start()
    {
        _distanceTraveled = 0;
        _oldPosition = transform.position;
        _randomStepSDistance = Random.Range(0.0f, 0.5f * stepDistance);
        puddles = GameObject.FindGameObjectsWithTag("Puddle");
    }
    
    void Update()
    {
        _distanceTraveled += (transform.position - _oldPosition).magnitude;
        _oldPosition = transform.position;
        if(_distanceTraveled >= stepDistance + _randomStepSDistance)
        {
            PlayFootstepSound();
            _distanceTraveled = 0;
            _randomStepSDistance = Random.Range(0.0f, 0.5f);
        }
        _oldPosition = transform.position;
    }

    private void PlayFootstepSound()
    {
        FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position - new Vector3(0,-0.5f,0)));

        ApplyParameters(eventInstance);
        
        eventInstance.start();
        eventInstance.release();
    }

    protected virtual void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        
    }
}
