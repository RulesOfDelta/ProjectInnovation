using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWallCollision : MonoBehaviour
{
    private WallTrigger _wallTrigger;
    
    void Start()
    {
        _wallTrigger = transform.parent.transform.parent.GetComponent<WallTrigger>();
    }

    void OnTriggerEnter(Collider collider){
        _wallTrigger.OnTriggerEnterChild(this.GetComponent<BoxCollider>());
    }
}
