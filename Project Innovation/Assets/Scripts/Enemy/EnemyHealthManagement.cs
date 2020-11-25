using UnityEngine;

public class EnemyHealthManagement : MonoBehaviour
{
    public int Health = 100;
    private CurrentStatus currentStatus;

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
        Debug.Log("Enemy received damage");
        Health -= damage;
        if(Health <= 0)
        {
            currentStatus = CurrentStatus.Dead;
            Destroy(gameObject);
        }
    }

    public CurrentStatus GetCurrentStatus()
    {
        return currentStatus;
    }
}