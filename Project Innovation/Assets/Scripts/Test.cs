using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
    }
}
