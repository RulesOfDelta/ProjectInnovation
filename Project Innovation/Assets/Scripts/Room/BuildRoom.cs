using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildNewRoom(Vector3 position, Vector3 scale, Quaternion rotation){
        Instantiate(_roomPrefab, position, rotation);
    }
}
