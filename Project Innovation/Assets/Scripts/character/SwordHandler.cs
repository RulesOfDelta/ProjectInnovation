using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    private List<Collider> attacked;

    private void Start()
    {
        attacked = new List<Collider>();
    }

    public void AfterAttack()
    {
        attacked.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!enemyMask.Contains(other.gameObject.layer) || attacked.Contains(other))
            return;
        // TODO attack enemy
        Debug.Log("Attacked Enemy");
        attacked.Add(other);
    }
}