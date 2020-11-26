using UnityEngine;

public class WalkingSound : MonoBehaviour
{
    private float _distanceTraveled;
    private Vector3 _oldPosition;
    public float stepDistance = 1.2f;
    private float _randomStepSDistance;

    [FMODUnity.EventRef]
    public string fmodEventPath;

    void Start()
    {
        _distanceTraveled = 0;
        _oldPosition = transform.position;
        _randomStepSDistance = Random.Range(0.0f, 0.5f);
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

        eventInstance.start();
        eventInstance.release();
    }
}
