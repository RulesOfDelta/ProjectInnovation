using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private float attackDist = 1f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private float damage;

    private void Start()
    {
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (gameObject)
        {
            if (Vector3.Distance(playerStats.transform.position, transform.position) < attackDist)
            {
                playerStats.Health -= damage;
                Debug.Log("Attack player");
                yield return new WaitForSeconds(attackTime);
            }
            else
                yield return null;
        }
    }
}
