using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private Collider m_rightHandCollider;
    [SerializeField]
    private Collider m_leftHandCollider;

    protected Animator m_animator;
    protected NavMeshAgent m_navMeshAgent;
    [HideInInspector]
    public UnitController m_targetCtrler;
    public UnitType UnitType;
    public UnitType TargetType;

    protected const float MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR = 0.8f;

    public float RotationSpeed = 10f;
    //public float _moveSpeed;
    //public float _hp = 100f;
    [SerializeField]
    private int m_damage = 10;

    static int UnitCount = 0;
    [HideInInspector]
    public int ID;

    protected UnitState m_currentState = UnitState.FindTarget;

    protected bool m_isAttacking = false;

    private void Awake()
    {
        UnitCount += 1;
        ID = UnitCount;

        m_rightHandCollider.enabled = false;
        m_leftHandCollider.enabled = false;
    }

    void Start()
    {
        m_animator = this.transform.GetComponent<Animator>();
        m_navMeshAgent = this.transform.GetComponent<NavMeshAgent>();

        DealDamage[] dealDmgs = transform.GetComponentsInChildren<DealDamage>();
        for (int i = 0; i < dealDmgs.Length; i++)
        {
            dealDmgs[i].SetDamage(m_damage);
            dealDmgs[i].SetUnitCtrler(this);
        }
    }

    void Update()
    {
        switch (m_currentState)
        {
            case UnitState.Idle:
                m_navMeshAgent.isStopped = false;
                m_animator.SetBool("CanWalk", false);
                break;
            case UnitState.FindTarget:
                m_targetCtrler = GameManager.Instance.GetClosesTarget(TargetType, transform.position);
                m_currentState = m_targetCtrler == null ? UnitState.Idle : UnitState.Run2Target;
                break;
            case UnitState.Run2Target:
                float distance = Vector3.Distance(m_navMeshAgent.transform.position, m_targetCtrler.transform.position);
                if (distance > MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR)
                {
                    m_navMeshAgent.isStopped = false;
                    m_navMeshAgent.destination = m_targetCtrler.transform.position;

                    m_animator.SetBool("CanWalk", true);
                }
                else // Reach target
                {
                    m_navMeshAgent.isStopped = true;
                    m_animator.SetBool("CanWalk", false);

                    m_currentState = UnitState.AttackTarget;
                }
                break;
            case UnitState.AttackTarget:
                float dis = Vector3.Distance(m_navMeshAgent.transform.position, m_targetCtrler.transform.position);
                if (dis > MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR)
                {
                    m_currentState = UnitState.Run2Target;
                }
                AttackTarget();
                break;
            case UnitState.KnockedOut:
                m_animator.SetTrigger("KnockedOut");
                break;
            case UnitState.InPool:
                break;
            case UnitState.Win:
                m_animator.SetTrigger("Victory");
                break;
            default:
                m_navMeshAgent.isStopped = false;
                m_animator.SetBool("CanWalk", false);
                break;
        }

        if (m_targetCtrler != null)
        {
            // Always look to target
            Vector3 direction = m_targetCtrler.transform.position - transform.position;
            direction.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
        }
    }

    public void OnAUnitKnockedOut(int theirUnitID)
    {
        if (m_targetCtrler != null && m_targetCtrler.ID != theirUnitID)
        {
            return;
        }

        m_targetCtrler = null;
        m_currentState = UnitState.FindTarget;
    }

    protected virtual void AttackTarget()
    {
    }

    public void OnAttackAnimationFinished()
    {
        m_isAttacking = false;
        m_animator.SetTrigger("AttackFinished");
    }

    public void OnKnockedOut()
    {
        m_currentState = UnitState.KnockedOut;
    }

    public void OnKnockedOutAnimationFinished()
    {
        GameManager.Instance.RecycleUnit(UnitType, gameObject);
        m_currentState = UnitState.InPool;
    }

    public void OnVictory()
    {
        m_currentState = UnitState.Win;
    }

    public void EnableRightHandCollider()
    {
        m_rightHandCollider.enabled = true;
    }

    public void DisableRightHandCollider()
    {
        m_rightHandCollider.enabled = false;
    }

    public void EnableLeftHandCollider()
    {
        m_leftHandCollider.enabled = true;
    }

    public void DisableLeftHandCollider()
    {
        m_leftHandCollider.enabled = false;
    }
}

public enum UnitState
{
    FindTarget = 1,
    Run2Target = 2,
    AttackTarget = 3,
    KnockedOut = 4,
    InPool = 5,
    Idle = 6,
    Win = 7,
}
