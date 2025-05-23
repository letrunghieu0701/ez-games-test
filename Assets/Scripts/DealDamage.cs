using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField]
    private float m_damage = 0;
    private UnitController m_unitCtrler;

    private void OnTriggerEnter(Collider other)
    {
        if (m_unitCtrler.TargetType == UnitType.Enemy && other.CompareTag("EnemyAI"))
        {
            UnitController otherUnitCtrler = other.GetComponent<UnitController>();
            if (!(m_unitCtrler.m_targetCtrler && m_unitCtrler.m_targetCtrler.ID == otherUnitCtrler.ID))
            {
                return;
            }

            HealthBar enemyHealth = other.GetComponent<HealthBar>();
            enemyHealth.TakeDamage(m_damage);
        }
        else if (m_unitCtrler.TargetType == UnitType.Player && other.CompareTag("Player"))
        {
            UnitController otherUnitCtrler = other.GetComponent<UnitController>();
            if (!(m_unitCtrler.m_targetCtrler && m_unitCtrler.m_targetCtrler.ID == otherUnitCtrler.ID))
            {
                return;
            }

            HealthBar enemyHealth = other.GetComponent<HealthBar>();
            enemyHealth.TakeDamage(m_damage);
        }
    }

    public void SetDamage(float value)
    {
        m_damage = value;
    }

    public void SetUnitCtrler(UnitController unitCtrler)
    {
        m_unitCtrler = unitCtrler;
    }
}
