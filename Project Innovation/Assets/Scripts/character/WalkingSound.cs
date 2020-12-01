using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingSound : MonoBehaviour
{
    private float _distanceTraveled;
    private Vector3 _oldPosition;
    public float stepDistance = 1.2f;
    private float _randomStepSDistance;
    protected GameObject[] puddles;

    [EventRef]
    public string fmodEventPath;

    private FMOD.Studio.EventInstance footstepInstance;

    void Start()
    {
        _distanceTraveled = 0;
        _oldPosition = transform.position;
        _randomStepSDistance = Random.Range(0.0f, 0.5f * stepDistance);
        // TODO doesn't work when regenerating
        puddles = GameObject.FindGameObjectsWithTag("Puddle");

        footstepInstance = fmodEventPath.CreateSound();
        footstepInstance.set3DAttributes((transform.position - new Vector3(0, -0.5f, 0)).To3DAttributes());
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

    private void OnDestroy()
    {
        footstepInstance.release();
    }

    private void PlayFootstepSound()
    {
        footstepInstance.set3DAttributes((transform.position - new Vector3(0, -0.5f, 0)).To3DAttributes());
        ApplyParameters(footstepInstance);
        
        footstepInstance.start();
    }

    protected virtual void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        
    }
}
