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
    private HealthBar m_healthBar;
    [HideInInspector]
    public UnitController m_targetCtrler;
    public UnitType UnitType;
    public UnitType TargetType;

    protected const float MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR = 0.8f;
    public float RotationSpeed = 10f;
    static int UnitCount = 0;
    [HideInInspector]
    public int ID;

    protected UnitState m_currentState = UnitState.FindTarget;
    protected bool m_isAttacking = false;

    private LevelData m_currentLevelData;
    private UnitBaseStatData m_baseStatData;

    private void Awake()
    {
        UnitCount += 1;
        ID = UnitCount;

        m_animator = this.transform.GetComponent<Animator>();
        m_navMeshAgent = this.transform.GetComponent<NavMeshAgent>();
        m_healthBar = this.transform.GetComponent<HealthBar>();

        DealDamage[] dealDmgs = transform.GetComponentsInChildren<DealDamage>();
        for (int i = 0; i < dealDmgs.Length; i++)
        {
            dealDmgs[i].SetUnitCtrler(this);
        }

        m_rightHandCollider.enabled = false;
        m_leftHandCollider.enabled = false;
    }

    void Start()
    {
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
                else // Reached target
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

            if (m_targetCtrler.GetUnitState() == UnitState.KnockedOut || m_targetCtrler.GetUnitState() == UnitState.InPool)
            {
                m_targetCtrler = null;
                m_currentState = UnitState.FindTarget;
            }
        }
    }

    public void SetData(LevelData levelData, UnitBaseStatData statData)
    {
        m_currentLevelData = levelData;
        m_baseStatData = statData;

        float scaleAtk = UnitType == UnitType.Player ? m_currentLevelData.ScaleAttackPlayer : m_currentLevelData.ScaleAttackEnemy;
        float scaleHp = UnitType == UnitType.Player ? m_currentLevelData.ScaleHealthPlayer : m_currentLevelData.ScaleHealthEnemy;
        float scaleSpeed = UnitType == UnitType.Player ? m_currentLevelData.ScaleMoveSpeedPlayer : m_currentLevelData.ScaleMoveSpeedEnemy;

        // Attack
        DealDamage[] dealDmgs = transform.GetComponentsInChildren<DealDamage>();
        for (int i = 0; i < dealDmgs.Length; i++)
        {
            dealDmgs[i].SetDamage(m_baseStatData.Attack * scaleAtk);
        }

        // HP
        m_healthBar.SetData(m_baseStatData.Health * scaleHp);

        // Move speed
        m_navMeshAgent.speed = m_baseStatData.MoveSpeed * scaleSpeed;

        m_currentState = UnitState.FindTarget;
        m_isAttacking = false;
    }

    public UnitState GetUnitState()
    {
        return m_currentState;
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
