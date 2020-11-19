using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 shotDirection = Vector3.zero;

    private void Update()
    {
        // TODO something more sophisticated (Rigidbody, etc.)
        transform.position += shotDirection * Time.deltaTime;
    }

    public void Fire(Vector3 direction)
    {
        shotDirection = direction.normalized * speed;
    }
}