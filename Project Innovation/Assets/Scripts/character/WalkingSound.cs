using UnityEngine;

public class WalkingSound : MonoBehaviour
{
    private float _distanceTraveled;
    private Vector3 _oldPosition;
    public float stepDistance = 1.2f;

    [FMODUnity.EventRef]
    public string fmodEventPath;

    void Start()
    {
        _distanceTraveled = 0;
        _oldPosition = transform.position;
    }
    
    void Update()
    {
        _distanceTraveled += (transform.position - _oldPosition).magnitude;
        if(_distanceTraveled >= stepDistance + Random.Range(0, 0.5f))
        {
            PlayFootstepSound();
            _distanceTraveled = 0;
        }
    }

    private void PlayFootstepSound()
    {
        FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventPath);
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        eventInstance.start();
        eventInstance.release();
    }
}
