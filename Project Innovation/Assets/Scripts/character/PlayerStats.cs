using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField] private string hitSoundSword, hitSoundShield;
    [SerializeField] private float maxHealth;
    [SerializeField] private MusicHandler musicHandler;
    private float health;

    private InputHandler controls;
    public bool shieldActive;
    public int shieldAngle = 90;

    public enum AttackMethod
    {
        Sword,
        Shield
    }

    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Min(value, maxHealth);
            if (health <= 0f) OnDeath();
            else musicHandler.HealthCallback(health);
        }
    }

    private void Start()
    {
        health = maxHealth;
        if (!controls)
            controls = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        controls.RegisterOnShield(ToggleShield);
    }

    private void OnDeath()
    {
        // TODO: DIE!!!!!!!!
        // Debug.Log("omfg he fuckin dedd");
    }

    public void Damage(float amount, AttackMethod attackMethod)
    {
        Health -= amount;
        Debug.Log("Damage player");
        if (Health > 0f)
        {
            // Play sound
            PlayHitSound(attackMethod);
        }
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

    private void PlayHitSound(AttackMethod attackMethod)
    {
        var attackSound = attackMethod == AttackMethod.Sword
            ? hitSoundSword.CreateSound()
            : hitSoundShield.CreateSound();
        attackSound.start();
        attackSound.release();
    }
}