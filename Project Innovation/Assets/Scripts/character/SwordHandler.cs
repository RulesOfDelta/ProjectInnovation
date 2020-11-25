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
        Debug.Log("this was exectued");
        // if(!other.gameObject.tag.Equals("GameController")) return;
        // Debug.Log("enemy was found");   
        // attacked.Add(other);
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