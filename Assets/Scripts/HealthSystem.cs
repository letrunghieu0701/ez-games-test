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

    private Camera m_camera;

    private void Start()
    {
        m_currentHealth = m_maxHealth;
        m_camera = Camera.main;
    }

    private void Update()
    {
        m_healthBarTrs.rotation = Quaternion.LookRotation(m_healthBarTrs.position - m_camera.transform.position);
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        UpdateHealthBar();
        Debug.Log("HP: " + m_currentHealth);
    }

    public void UpdateHealthBar()
    {
        m_healthBarSpriteForeground.fillAmount = m_currentHealth / m_maxHealth;
    }
}
