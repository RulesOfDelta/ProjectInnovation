using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private float attackDistance = 5.0f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float damage;
    Color debugLineColor = Color.red;
    public bool showDebugInfo = true;

    private void Start()
    {
        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (showDebugInfo && playerStats.shieldActive)
        {
            Vector3 differenceVector = transform.position - playerStats.transform.position;
            Debug.DrawLine(transform.position,playerStats.transform.position, debugLineColor);
            float angle = Vector3.Angle(differenceVector, playerStats.transform.forward);
            if (angle > playerStats.shieldAngle) debugLineColor = Color.red; 
            else debugLineColor = Color.green;
            
        }    
    }
    
    public IEnumerator AttackLoop()
    {
        while (gameObject)
        {
            if (Vector3.Distance(playerStats.transform.position, transform.position) < attackDistance)
            {
                //Is player holding up the shield?
                if (playerStats.shieldActive)
                {
                    Vector3 differenceVector = transform.position - playerStats.transform.position;
                    float angle = Vector3.Angle(differenceVector, playerStats.transform.forward);
                    if (angle > playerStats.shieldAngle) playerStats.Health -= damage;
                }
                else playerStats.Health -= damage;
                yield return new WaitForSeconds(attackInterval);
            }
            else
                yield return null;
        }
    }
}