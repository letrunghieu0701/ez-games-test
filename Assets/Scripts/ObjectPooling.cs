using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling
{
    private static ObjectPooling instance = null;

    private ObjectPooling() { }

    public static ObjectPooling Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPooling();
            }

            return instance;
        }
    }

    private Transform m_poolContainer;

    private Queue<GameObject> m_playerUnitPool = new Queue<GameObject>();
    private Queue<GameObject> m_enemyUnitPool = new Queue<GameObject>();

    private GameObject m_playerUnit;
    private GameObject m_enemyUnit;

    private readonly Vector3 VERY_FAR_AWAY_POS = new Vector3(1000, 1000, 1000);

    public void Init(int numPlayerUnit, GameObject playerUnit, int numEnemyUnit, GameObject enemyUnit)
    {
        m_poolContainer = new GameObject().transform;
        m_poolContainer.gameObject.name = "ObjectPool";
        m_poolContainer.position = VERY_FAR_AWAY_POS;

        m_playerUnit = playerUnit;
        m_enemyUnit = enemyUnit;

        for (int i = 0; i < numPlayerUnit; i++)
        {
            GameObject unit = GameObject.Instantiate(playerUnit, Vector3.zero, Quaternion.identity, m_poolContainer);
            UnitController unitCtrler = unit.GetComponent<UnitController>();
            unit.name = $"{unit.name} {unitCtrler.ID}";
            unit.SetActive(false);

            m_playerUnitPool.Enqueue(unit);
        }

        for (int i = 0; i < numEnemyUnit; i++)
        {
            GameObject unit = GameObject.Instantiate(enemyUnit, Vector3.zero, Quaternion.identity, m_poolContainer);
            unit.SetActive(false);

            m_enemyUnitPool.Enqueue(unit);
        }
    }

    public GameObject GetUnit(UnitType unitType)
    {
        GameObject prefab = m_enemyUnit;
        Queue<GameObject> unitPool = m_enemyUnitPool;
        if (unitType == UnitType.Player)
        {
            prefab = m_playerUnit;
            unitPool = m_playerUnitPool;
        }

        GameObject unit;
        if (unitPool.Count == 0)
        {
            unit = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, m_poolContainer);
        }
        else
        {
            unit = unitPool.Dequeue();
        }

        return unit;
    }

    public void RecycleUnit(UnitType unitType, GameObject unit)
    {
        unit.SetActive(false);
        unit.transform.position = Vector3.zero;
        if (unitType == UnitType.Player)
        {
            m_playerUnitPool.Enqueue(unit);
        }
        else if (unitType == UnitType.Enemy)
        {
            m_enemyUnitPool.Enqueue(unit);
        }
    }
}
