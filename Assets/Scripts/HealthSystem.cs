using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private float m_maxHealth = 100f;
    private float m_currentHealth;

    [SerializeField]
    private Image m_healthBarSpriteForeground;
    [SerializeField]
    private Transform m_healthBarTrs;
    [SerializeField]
    private float m_reduceHealhAnimationSpeed = 2f;
    private float m_newFillAmount = 1;

    private Camera m_camera;

    private void Start()
    {
        m_currentHealth = m_maxHealth;
        m_camera = Camera.main;
    }

    private void Update()
    {
        m_healthBarTrs.rotation = Quaternion.LookRotation(m_healthBarTrs.position - m_camera.transform.position);
        m_healthBarSpriteForeground.fillAmount = Mathf.MoveTowards(m_healthBarSpriteForeground.fillAmount, m_newFillAmount, m_reduceHealhAnimationSpeed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        UpdateHealthBar();
        Debug.Log("HP: " + m_currentHealth);
    }

    public void UpdateHealthBar()
    {
        m_newFillAmount = m_currentHealth / m_maxHealth;
    }
}
