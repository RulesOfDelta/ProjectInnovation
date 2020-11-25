using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private InputHandler handler;
    [Header("Sword settings")]
    [SerializeField] private SwordHandler sword;
    [SerializeField] private float swordAttackTime = 0.1f;
    [SerializeField] private float swordAttackCooldown = 1f;

    private bool attacking = false;

    private void Start()
    {
        sword.gameObject.SetActive(false);
        handler.RegisterOnFire(OnFire);
        handler.RegisterOnSword(OnSword);
    }
    
    private void OnDestroy()
    {
        handler.DeregisterOnFire(OnFire);
        handler.DeregisterOnSword(OnSword);
    }

    private void OnFire()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletParent)
            .GetComponent<Bullet>();
        if(bullet) bullet.Fire(transform.forward);
        else Debug.LogError("Bullet prefab should have a Bullet component on it");
    }

    private void OnSword()
    {
        // TODO: spawn hitbox that stays in front of player and that does damage once to the enemy
        SwordAttack();
    }

    private void SwordAttack()
    {
        if (attacking) return;
        sword.gameObject.SetActive(true);
        attacking = true;
        StartCoroutine(Disable());

        Debug.Log("Attack");

        IEnumerator Disable()
        {
            yield return new WaitForSeconds(swordAttackTime);
            sword.gameObject.SetActive(false);
            sword.AfterAttack();
            yield return new WaitForSeconds(swordAttackCooldown);
            attacking = false;
        }
    }
}
