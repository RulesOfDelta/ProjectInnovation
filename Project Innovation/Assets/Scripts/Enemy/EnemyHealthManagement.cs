using UnityEngine;

public class EnemyHealthManagement : MonoBehaviour
{
    public int Health = 100;
    private CurrentStatus currentStatus;

    [FMODUnity.EventRef, SerializeField] private string damageSound;
    [FMODUnity.EventRef, SerializeField] private string deathSound;

    public enum CurrentStatus
    {
        Alive,
        Dead
    }

    private void Start()
    {
        currentStatus = CurrentStatus.Alive;
    }

    public int GetHealth()
    {
        return Health;
    }

    public void SetHealth(int newHealth)
    {
        if (newHealth > 0) Health = newHealth;
    }

    public void ReduceHealth(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            PlaySound(deathSound);
            currentStatus = CurrentStatus.Dead;
            Destroy(gameObject);
        }
        else
        {
            PlaySound(damageSound);
        }
    }

    public CurrentStatus GetCurrentStatus()
    {
        return currentStatus;
    }

    private void PlaySound(string path)
    {
        FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(path);
        eventInstance.set3DAttributes(
            FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        eventInstance.start();
        eventInstance.release();
    }
}