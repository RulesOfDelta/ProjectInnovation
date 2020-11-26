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

    [FMODUnity.EventRef, SerializeField] private string attackSound;

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
                PlayAttackSound();
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
                        if (angle > playerStats.shieldAngle) playerStats.Damage(damage);
                    }
                    else playerStats.Damage(damage);
                }
                yield return new WaitForSeconds(attackInterval);
            }
            else
                yield return null;
        }
    }

    private void PlayAttackSound()
    {
        // TODO maybe cache a single instance (does that work?)
        var eventInstance = FMODUnity.RuntimeManager.CreateInstance(attackSound);
        eventInstance.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform.position - new Vector3(0, -0.5f, 0)));

        eventInstance.start();
        eventInstance.release();
    }
}