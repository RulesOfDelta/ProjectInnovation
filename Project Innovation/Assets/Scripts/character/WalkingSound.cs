using UnityEngine;
using UnityEngine.UIElements;

public class WalkingSound : MonoBehaviour
{
    private float _distanceTraveled;
    private Vector3 _oldPosition;
    public float stepDistance = 1.2f;
    private float _randomStepSDistance;

    [FMODUnity.EventRef]
    public string fmodEventPath;

    private FMOD.Studio.EventInstance footStepInstance;

    void Start()
    {
        _distanceTraveled = 0;
        _oldPosition = transform.position;
        _randomStepSDistance = Random.Range(0.0f, 0.5f);
        
        footStepInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
        footStepInstance.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(_oldPosition - new Vector3(0, -0.5f, 0)));
    }

    private void OnDestroy()
    {
        footStepInstance.release();
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
        footStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position - new Vector3(0,-0.5f,0)));

        ApplyParameters(footStepInstance);
        
        footStepInstance.start();
    }

    protected virtual void ApplyParameters(FMOD.Studio.EventInstance eventInstance)
    {
        
    }
}
