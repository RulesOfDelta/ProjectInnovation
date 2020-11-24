using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health;
    
    public InputHandler controls;
    public bool shieldActive;
    public int shieldAngle = 90;
    
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
        controls.RegisterOnShield(ToggleShield);
    }

    private void OnDeath()
    {
        // TODO: DIE!!!!!!!!
    }
    
    private void ToggleShield(InputHandler.ButtonAction action)
    {
        if (action == InputHandler.ButtonAction.Down)
        {
            Debug.Log("Shield active");
            shieldActive = true;
        } 
        else if (action == InputHandler.ButtonAction.Up)
        {
            Debug.Log("Shield inactive");
            shieldActive = false;
        }
    }
}
