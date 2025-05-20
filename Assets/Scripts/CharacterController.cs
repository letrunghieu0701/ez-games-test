using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_navMeshAgent;
    public Transform Enemy;

    [SerializeField]
    private Collider m_rightHandCollider;
    [SerializeField]
    private Collider m_leftHandCollider;

    private const float MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR = 0.63f;

    public float RotationSpeed = 10f;
    public float _moveSpeed;
    //public float _hp = 100f;
    public float _damage = 10f;

    void Start()
    {
        m_animator = this.transform.GetComponent<Animator>();
        m_navMeshAgent = this.transform.GetComponent<NavMeshAgent>();

        m_rightHandCollider.enabled = false;
        m_leftHandCollider.enabled = false;
    }

    void Update()
    {
        // Handle attack inputs
        if (Input.GetKeyDown(KeyCode.J))
        {
            m_animator.SetTrigger("punchLeft");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_animator.SetTrigger("punchRight");
        }

        // Always look to target
        Vector3 direction = Enemy.position - transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);

        // Walk to target
        float distance = Vector3.Distance(m_navMeshAgent.transform.position, Enemy.position);
        //Debug.Log("Distance: " + distance);
        if (distance <= MIN_ATTACK_DISTANCE_BETWEEN_2_CHAR)
        {

            m_navMeshAgent.isStopped = true;
            m_animator.SetBool("Walk", false);
            // attack = true
        }
        else
        {
            m_navMeshAgent.isStopped = false;
            m_navMeshAgent.destination = Enemy.position;

            m_animator.SetBool("Walk", true);
            // attack = false
        }
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
