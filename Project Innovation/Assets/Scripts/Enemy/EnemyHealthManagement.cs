using UnityEngine;

public class EnemyHealthManagement : MonoBehaviour
{
    private int health;
    private CurrentStatus currentStatus;

    public enum CurrentStatus
    {
        Alive,
        Dead
    }

    private void Start()
    {
        health = 100;
        currentStatus = CurrentStatus.Alive;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int newHealth)
    {
        if (newHealth > 0) health = newHealth;
    }

    public void ReduceHealth(int damage)
    {
        Debug.Log("Enemy received damage");
        if (health > 0) health -= damage;
        else currentStatus = CurrentStatus.Dead;
        Destroy(gameObject);
    }

    public CurrentStatus GetCurrentStatus()
    {
        return currentStatus;
    }
}