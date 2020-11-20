using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 shotDirection = Vector3.zero;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask boundsLayer;
    [SerializeField, FMODUnity.EventRef] private string hitPath;

    private void Update()
    {
        // TODO something more sophisticated (Rigidbody, etc.)
        transform.position += shotDirection * Time.deltaTime;
    }

    public void Fire(Vector3 direction)
    {
        shotDirection = direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyLayer.Contains(other.gameObject.layer)) PlayHitSound();
    }

    private void OnTriggerExit(Collider other)
    {
        if(boundsLayer.Contains(other.gameObject.layer)) Destroy(gameObject);
    }

    private void PlayHitSound()
    {
        var e = FMODUnity.RuntimeManager.CreateInstance(hitPath);
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        e.start();
        e.release();
        Destroy(gameObject);
    }
}