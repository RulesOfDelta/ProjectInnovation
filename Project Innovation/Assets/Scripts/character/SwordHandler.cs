using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private int swordDamage = 50;

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
        if (!enemyMask.Contains(other.gameObject.layer) || attacked.Contains(other)) return;
        attacked.Add(other);
        AddDamageToEnemies(other.gameObject);
    }

    private void AddDamageToEnemies(GameObject hitGameObject)
    {
        hitGameObject.GetComponent<EnemyHealthManagement>().ReduceHealth(swordDamage);
        Highscore.AddToHighscore(20);
    }
}