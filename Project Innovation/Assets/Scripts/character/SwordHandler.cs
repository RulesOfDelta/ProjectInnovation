using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private int swordDamage = 5;

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
        if(!enemyMask.Contains(other.gameObject.layer) || attacked.Contains(other)) return;
        attacked.Add(other);
        AddDamageToEnemies();
    }

    private void AddDamageToEnemies()
    {
        foreach (Collider currentCollider in attacked)
        {
            currentCollider.GetComponent<EnemyHealthManagement>().ReduceHealth(swordDamage);
        }
    }
}