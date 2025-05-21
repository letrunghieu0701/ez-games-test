using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{
    protected override void AttackTarget()
    {
        // Only allow attacking when the previous attack has been done
        if (base.m_isAttacking)
        {
            return;
        }

        // Handle attack inputs
        if (Input.GetKeyDown(KeyCode.J))
        {
            base.m_animator.SetTrigger("PunchLeft");
            base.m_isAttacking = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            base.m_animator.SetTrigger("PunchRight");
            base.m_isAttacking = true;
        }
    }
}
