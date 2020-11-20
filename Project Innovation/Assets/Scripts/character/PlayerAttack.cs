using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private SwordHandler sword;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private InputHandler handler;

    private void Start()
    {
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
        sword.Attack();
    }
}
