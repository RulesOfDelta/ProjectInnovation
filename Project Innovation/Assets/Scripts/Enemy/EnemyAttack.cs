using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private float attackDistance = 5.0f;
    [SerializeField] private float chargeTime = 0.2f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float damage;
    Color debugLineColor = Color.red;
    public bool showDebugInfo = true;
    // TODO maybe load event at start?

    [SerializeField] private bool hasAttackSound;
    [FMODUnity.EventRef, SerializeField] private string attackSound;
    private FMOD.Studio.EventInstance attackInstance;
    [FMODUnity.EventRef, SerializeField] private string prepareSound;
    private FMOD.Studio.EventInstance prepareInstance;

    private void Start()
    {
        if(hasAttackSound)
            attackInstance = FMODUnity.RuntimeManager.CreateInstance(attackSound);
        prepareInstance = FMODUnity.RuntimeManager.CreateInstance(prepareSound);
        
        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (showDebugInfo && playerStats.shieldActive)
        {
            Vector3 differenceVector = transform.position - playerStats.transform.position;
            Debug.DrawLine(transform.position,playerStats.transform.position, debugLineColor);
            float angle = Vector3.Angle(differenceVector, playerStats.transform.forward);
#if UNITY_EDITOR
            if (angle > playerStats.shieldAngle) debugLineColor = Color.red; 
            else debugLineColor = Color.green;
#endif
        }
    }
    
    public IEnumerator AttackLoop()
    {
        while (gameObject)
        {
            if (Vector3.Distance(playerStats.transform.position, transform.position) < attackDistance)
            {
                prepareInstance.PlayAtPos(transform.position);
                yield return new WaitForSeconds(chargeTime);
                Debug.Log("Try attack player");
                // TODO stop enemy during this time
                if (Vector3.Distance(playerStats.transform.position, transform.position) < attackDistance)
                {
                    // Check again, player might not be in range anymore
                    // Play attack sound
                    //Is player holding up the shield?
                    if (playerStats.shieldActive)
                    {
                        Vector3 differenceVector = transform.position - playerStats.transform.position;
                        float angle = Vector3.Angle(differenceVector, playerStats.transform.forward);
                        if (angle > playerStats.shieldAngle)
                        {
                            playerStats.Damage(damage, PlayerStats.AttackMethod.Sword);
                            attackInstance.PlayAtPos(transform.position);
                        }
                    }
                    else
                    {
                        playerStats.Damage(damage, PlayerStats.AttackMethod.Shield);
                        if(hasAttackSound)
                             attackInstance.PlayAtPos(transform.position);
                    }
                }
                yield return new WaitForSeconds(attackInterval);
            }
            else
                yield return null;
        }
    }
}