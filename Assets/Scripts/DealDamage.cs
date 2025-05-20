using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField]
    private float m_damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAI")
            || other.CompareTag("Player")
            || other.CompareTag("FriendlyAI"))
        {
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            enemyHealth.TakeDamage(m_damage);
        }
    }
}
