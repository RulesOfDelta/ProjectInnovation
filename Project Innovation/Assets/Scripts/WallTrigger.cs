using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    void Start()
    { }

    void Update()
    { }

    public void OnTriggerEnterChild(Collider childCollider){
        Debug.Log("Collider entered the trigger!");
    }
}
