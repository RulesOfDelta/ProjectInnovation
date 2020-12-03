using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRemoveNotifier : MonoBehaviour
{
    public delegate void Evnt(GameObject obj);
    private event Evnt Notify;

    private void OnDestroy()
    {
        Notify?.Invoke(gameObject);
    }
    
    public void AddEvent(Evnt e)
    {
        Notify += e;
    }

    public void RemoveEvent(Evnt e)
    {
        Notify -= e;
    }
}
