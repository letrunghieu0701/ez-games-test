using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : UnitController
{
    protected override void AttackTarget()
    {
        // Only allow attacking when the previous attack has been done
        if (base.m_isAttacking)
        {
            return;
        }

        base.m_isAttacking = true;
        // Random attack
        int random = Random.Range(0, 100);
        if (random > 50)
        {
            base.m_animator.SetTrigger("PunchLeft");
        }
        else
        {
            base.m_animator.SetTrigger("PunchRight");
        }
    }
}
