using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private int swordDamage = 50;
    [EventRef, SerializeField] private string swordHitSound;
    private EventInstance swordHitInstance;

    private List<Collider> attacked;

    private void Start()
    {
        attacked = new List<Collider>();
        swordHitInstance = swordHitSound.CreateSound();
    }

    public void AfterAttack()
    {
        attacked.Clear();
    }

    private void OnDestroy()
    {
        swordHitInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        swordHitInstance.release();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enemyMask.Contains(other.gameObject.layer) || attacked.Contains(other)) return;
        attacked.Add(other);
        AddDamageToEnemies(other.gameObject);
    }

    private void AddDamageToEnemies(GameObject hitGameObject)
    {
        swordHitInstance.start();
        hitGameObject.GetComponent<EnemyHealthManagement>().ReduceHealth(swordDamage);
        Highscore.AddToHighscore(20);
    }
}