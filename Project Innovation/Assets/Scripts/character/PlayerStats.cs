using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField]
    private string deathSound, hitSoundSword, hitSoundShield, moveShieldUp, moveShieldDown, hiddenSound;

    [SerializeField] private float maxHealth;
    [SerializeField] private MusicHandler musicHandler;
    private PlayerMusicHandler playerMusicHandler;
    private float health;

    private InputHandler controls;
    public bool shieldActive;
    public int shieldAngle = 90;

    //Very important
    public InputHandler test;

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
        controls.RegisterOnHidden(FatAssFart);
        playerMusicHandler = GetComponent<PlayerMusicHandler>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Health = -1337;
        }
#endif
    }

    private void OnDeath()
    {
        playerMusicHandler.ApplyDeathFilter();
        playerMusicHandler.StopHeartbeat();

        GameObject.Find("Room").GetComponent<EnemySpawner>().ClearEnemies();
        
        Highscore.SaveCurrentHighscore();
        
        RuntimeManager.PlayOneShot(deathSound, Vector3.zero);
        StartCoroutine(Hack());
        
        controls.HaltInputUntilFire(3.0f, Resurrect);

        IEnumerator Hack()
        {
            yield return null;
            yield return null;
            yield return null;
            musicHandler.State = MusicHandler.MusicState.Stop;
        }
    }

    private void Resurrect()
    {
        Health = maxHealth;
        playerMusicHandler.RemoveDeathFilter();
        GameObject.Find("Room").GetComponent<Room2>().Generate();
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
            var moveShieldUpSound = moveShieldUp.CreateSound();
            moveShieldUpSound.start();
            shieldActive = true;
        }
        else if (action == InputHandler.ButtonAction.Up)
        {
            var moveShieldDownSound = moveShieldDown.CreateSound();
            moveShieldDownSound.start();
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

    private void FatAssFart()
    {
        var hiddenSoundInstance = hiddenSound.CreateSound();
        hiddenSoundInstance.start();
    }
}