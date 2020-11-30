using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private float attackDistance = 5.0f;
    [SerializeField] private float chargeTime = 0.2f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float damage;
    Color debugLineColor = Color.red;
    public float heartbeatRadius = 4.0f;

    public bool showDebugInfo = true;
    // TODO maybe load event at start?

    [SerializeField] private bool hasAttackSound;
    [FMODUnity.EventRef, SerializeField] private string attackSound;
    private FMOD.Studio.EventInstance attackInstance;
    [FMODUnity.EventRef, SerializeField] private string prepareSound;
    private FMOD.Studio.EventInstance prepareInstance;

    private int enemiesWithinHeartbeatRadius;
    private bool isWithinHeartbeatRadius;

    private void Start()
    {
        if (hasAttackSound)
            attackInstance = attackSound.CreateSound();
        prepareInstance = prepareSound.CreateSound();

        attackInstance.set3DAttributes(transform.position.To3DAttributes());
        prepareInstance.set3DAttributes(transform.position.To3DAttributes());

        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        enemiesWithinHeartbeatRadius = 0;
        isWithinHeartbeatRadius = false;
    }

    private void Update()
    {
        CheckForHeartbeatDistance();
        if (showDebugInfo && playerStats.shieldActive)
        {
            Vector3 differenceVector = transform.position - playerStats.transform.position;
            Debug.DrawLine(transform.position, playerStats.transform.position, debugLineColor);
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
            float distanceToPlayer = Vector3.Distance(playerStats.transform.position, transform.position);

            if (distanceToPlayer < attackDistance)
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
                            Highscore.ReduceHighscore(10);
                        }
                        else
                        {
                            playerStats.Damage(0, PlayerStats.AttackMethod.Shield);
                        }
                    }
                    else
                    {
                        playerStats.Damage(damage, PlayerStats.AttackMethod.Sword);
                        if (hasAttackSound) attackInstance.PlayAtPos(transform.position);
                        Highscore.ReduceHighscore(15);
                    }
                }

                yield return new WaitForSeconds(attackInterval);
            }
            else
                yield return null;
        }
    }


    private void CheckForHeartbeatDistance()
    {
        float distanceToPlayer = Vector3.Distance(playerStats.transform.position, transform.position);
        if (distanceToPlayer <= heartbeatRadius && !isWithinHeartbeatRadius)
        {
            if(enemiesWithinHeartbeatRadius == 0) playerStats.gameObject.GetComponent<PlayerMusicHandler>().PlayHeartbeat(distanceToPlayer);
            enemiesWithinHeartbeatRadius++;
            isWithinHeartbeatRadius = true;
        }
        else if (distanceToPlayer > heartbeatRadius && isWithinHeartbeatRadius)
        {
            enemiesWithinHeartbeatRadius--;
            isWithinHeartbeatRadius = false;
            if(enemiesWithinHeartbeatRadius == 0) playerStats.gameObject.GetComponent<PlayerMusicHandler>().StopHeartbeat();
        }
    }
}