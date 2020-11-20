using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackTime = 0.1f;

    [SerializeField] private LayerMask enemyMask;

    private List<Collider> attacked;
    private bool attacking;

    private void Start()
    {
        gameObject.SetActive(false);
        attacked = new List<Collider>();
        attacking = false;
    }

    public void Attack()
    {
        if (attacking) return;
        attacking = true;
        gameObject.SetActive(true);
        StartCoroutine(Disable());

        IEnumerator Disable()
        {
            yield return new WaitForSeconds(attackTime);
            gameObject.SetActive(false);
            attacked.Clear();
            attacking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enemyMask.Contains(other.gameObject.layer) || attacked.Contains(other)) return;
        // TODO attack enemy
        Debug.Log("Attacked Enemy");
        attacked.Add(other);
    }
}