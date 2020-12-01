using UnityEngine;
using FMODUnity;

public class MovingSound : MonoBehaviour
{
    [EventRef, SerializeField] private string soundPath;
    private FMOD.Studio.EventInstance soundInstance;
    [SerializeField, Min(0.01f)] private float minSpeed;
    [SerializeField, Min(0.01f)] private float maxSpeed;
    private float speed;

    private Vector3 startPoint;
    private Vector3 bounds;

    private Vector3 waypoint;

    public void SetBounds(Vector3 newStart, Vector3 newBounds)
    {
        startPoint = newStart;
        bounds = newBounds;

        transform.position = GetRandomPos();
        waypoint = GetRandomPos();
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Start()
    {
        soundInstance = soundPath.CreateSound();
        soundInstance.set3DAttributes(transform.position.To3DAttributes());
        soundInstance.start();
        waypoint = GetRandomPos();
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void OnDestroy()
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        soundInstance.release();
    }

    private void Update()
    {
        transform.position += (waypoint - transform.position).normalized * (speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, waypoint) < speed * Time.deltaTime * 3f)
        {
            waypoint = GetRandomPos();
        }
        
        soundInstance.set3DAttributes(transform.position.To3DAttributes());
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(0f, bounds.x), Random.Range(0f, bounds.y), Random.Range(0f, bounds.z))
            - bounds / 2f + startPoint;
    }
}
