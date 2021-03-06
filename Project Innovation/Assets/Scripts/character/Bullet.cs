using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 shotDirection = Vector3.zero;

    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask boundsLayer;
    [EventRef, SerializeField] private string hitSound;

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
        if (enemyLayer.Contains(other.gameObject.layer))
        {
            var stats = other.GetComponent<EnemyHealthManagement>();
            if (stats)
            {
                stats.ReduceHealth(damage);
                Destroy(gameObject);
                RuntimeManager.PlayOneShot(hitSound, transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(boundsLayer.Contains(other.gameObject.layer)) Destroy(gameObject);
    }
}