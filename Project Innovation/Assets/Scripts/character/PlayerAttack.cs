using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletParent;
    private InputHandler handler;

    [Header("Shooty settings")] [SerializeField]
    private float gunCooldown = 3f;
    [Header("Sword settings")]
    [SerializeField] private SwordHandler sword;
    [SerializeField] private float swordAttackTime = 0.1f;
    [SerializeField] private float swordAttackCooldown = 1f;

    private enum Weapon
    {
        Sword,
        Bow
    }

    private Weapon currentWeapon = Weapon.Sword;

    private bool attacking;
    
    [FMODUnity.EventRef]
    public string swordEventPath;

    private FMOD.Studio.EventInstance swordSound;
    

    private void Start()
    {
        sword.gameObject.SetActive(false);
        if (!handler)
            handler = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        handler.RegisterOnFire(OnFire);
        handler.RegisterOnSwitch(OnSwitch);
        swordSound = FMODUnity.RuntimeManager.CreateInstance(swordEventPath);
    }
    
    private void OnDestroy()
    {
        handler.DeregisterOnFire(OnFire);
        handler.DeregisterOnSwitch(OnSwitch);
    }

    private bool canShoot = true;

    private void OnFire()
    {
        switch (currentWeapon)
        {
            case Weapon.Sword:
                SwordAttack();
                break;
            case Weapon.Bow:
                BowAttack();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void BowAttack()
    {
        if (!canShoot) return;
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletParent)
            .GetComponent<Bullet>();
        if (bullet)
        {
            bullet.Fire(transform.forward);
            StartCoroutine(ShootCooldown());
        }
        else Debug.LogError("Bullet prefab should have a Bullet component on it");
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(gunCooldown);
        canShoot = true;
    }

    private void OnSwitch()
    {
        switch (currentWeapon)
        {
            case Weapon.Sword:
                currentWeapon = Weapon.Bow;
                break;
            case Weapon.Bow:
                currentWeapon = Weapon.Sword;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SwordAttack()
    {
        if (attacking) return;
        swordSound.start();
        sword.gameObject.SetActive(true);
        attacking = true;
        StartCoroutine(Disable());

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
