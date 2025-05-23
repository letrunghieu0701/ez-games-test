using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float m_maxHealth;
    public float m_currentHealth;

    [SerializeField]
    private Image m_healthBarSpriteForeground;
    [SerializeField]
    private Transform m_healthBarTrs;
    [SerializeField]
    private float m_reduceHealhAnimationSpeed = 2f;
    private float m_newFillAmount = 1;

    private Camera m_camera;

    private UnitController m_unit;

    private void Start()
    {
        m_camera = Camera.main;
        m_unit = this.transform.GetComponent<UnitController>();
    }

    private void Update()
    {
        m_healthBarTrs.rotation = Quaternion.LookRotation(m_healthBarTrs.position - m_camera.transform.position);
        if (m_healthBarSpriteForeground.fillAmount > m_newFillAmount)
        {
            m_healthBarSpriteForeground.fillAmount = Mathf.MoveTowards(m_healthBarSpriteForeground.fillAmount, m_newFillAmount, m_reduceHealhAnimationSpeed * Time.deltaTime);
        }
    }

    public void SetData(float maxHealth)
    {
        m_maxHealth = maxHealth;
        m_currentHealth = m_maxHealth;
        UpdateHealthBar();
        m_healthBarSpriteForeground.fillAmount = 1;
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth - damage, 0, m_currentHealth);
        UpdateHealthBar();
        if (m_currentHealth == 0)
        {
            BoxCollider bodyCollider = this.transform.GetComponent<BoxCollider>();
            bodyCollider.enabled = false;
            SetActiveHealhBar(false);
            GameManager.Instance.NotifyAUnitKnockedOut(m_unit.UnitType, m_unit.ID);
            m_unit.OnKnockedOut();
        }
    }

    public void UpdateHealthBar()
    {
        m_newFillAmount = m_currentHealth / m_maxHealth;
    }

    public void SetActiveHealhBar(bool value)
    {
        m_healthBarTrs.gameObject.SetActive(value);
    }
}
