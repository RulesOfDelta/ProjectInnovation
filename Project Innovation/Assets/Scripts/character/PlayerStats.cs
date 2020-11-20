using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health;

    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Min(value, maxHealth);
            if(health <= 0f) OnDeath();
        }
    }

    private void Start()
    {
        health = maxHealth;
    }

    private void OnDeath()
    {
        // TODO: DIE!!!!!!!!
    }
}
