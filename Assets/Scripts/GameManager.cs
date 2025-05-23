using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameManager() { }

    [SerializeField]
    private GameObject m_playerPrefab;
    [SerializeField]
    private int m_initPlayerUnitPoolingNum;
    [SerializeField]
    private GameObject m_enemyPrefab;
    [SerializeField]
    private int m_initEnemyUnitPoolingNum;

    [SerializeField]
    private List<Transform> m_playerSpawnPos;
    [SerializeField]
    private List<Transform> m_enemySpawnPos;

    private Dictionary<int, GameObject> m_playerUnits = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_enemyUnits = new Dictionary<int, GameObject>();

    public UnitBaseStatDataSO m_unitBaseStatDataSO;
    public LevelDataSO m_levelDataSO;
    private LevelData m_currentLevelData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        ObjectPooling.Instance.Init(m_initPlayerUnitPoolingNum, m_playerPrefab, m_initEnemyUnitPoolingNum, m_enemyPrefab);
        UIManager.Instance.ShowUI("UIMenu");
    }

    void Update()
    {
    }

    public void LoadGameMode(int gameLevel)
    {
        CleanBattleStage();
        m_currentLevelData = m_levelDataSO.Levels[gameLevel - 1];
        switch ((GameMode)m_currentLevelData.GameMode)
        {
            case GameMode.OneVsOne:
                GetUnit(UnitType.Player);
                GetUnit(UnitType.Enemy);
                break;
            case GameMode.OneVsMany:
                GetUnit(UnitType.Player);

                for (int i = 0; i < m_currentLevelData.EnemyUnitNum; i++)
                {
                    GetUnit(UnitType.Enemy);
                }
                break;
            case GameMode.ManyVsMany:
                for (int i = 0; i < m_currentLevelData.PlayerUnitNum; i++)
                {
                    GetUnit(UnitType.Player);
                }

                for (int i = 0; i < m_currentLevelData.EnemyUnitNum; i++)
                {
                    GetUnit(UnitType.Enemy);
                }
                break;
        }

        UIManager.Instance.HideAllUI(); ;
    }

    private void GetUnit(UnitType unitType)
    {
        Dictionary<int, GameObject> units = m_enemyUnits;
        UnitBaseStatData statData = m_unitBaseStatDataSO.Stats[1];
        List<Transform> spawnPos = m_enemySpawnPos;
        if (unitType == UnitType.Player)
        {
            units = m_playerUnits;
            spawnPos = m_playerSpawnPos;
            statData = m_unitBaseStatDataSO.Stats[0];
        }

        // Get unit
        GameObject unit = ObjectPooling.Instance.GetUnit(unitType);
        unit.SetActive(true);
        UnitController unitCtrler = unit.GetComponent<UnitController>();
        Vector3 randomOffset = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f));
        unit.transform.position = spawnPos[unitCtrler.ID % spawnPos.Count].position + randomOffset;

        // Reset health bar and collider
        HealthBar unitHealthSys = unit.GetComponent<HealthBar>();
        unitHealthSys.SetActiveHealhBar(true);
        BoxCollider bodyCollider = unit.GetComponent<BoxCollider>();
        bodyCollider.enabled = true;

        // Set data
        units.Add(unitCtrler.ID, unit);
        unitCtrler.SetData(m_currentLevelData, statData);
    }

    public UnitController GetClosesTarget(UnitType targetType, Vector3 myPos)
    {
        Dictionary<int, GameObject> unitsByKey = m_enemyUnits;
        if (targetType == UnitType.Player)
        {
            unitsByKey = m_playerUnits;
        }

        int closetUnitID = -1;
        float closetDistance = float.MaxValue;
        foreach (var item in unitsByKey)
        {
            float distance = (myPos - item.Value.transform.position).sqrMagnitude;
            if (distance < closetDistance)
            {
                closetDistance = distance;
                closetUnitID = item.Key;
            }
        }

        GameObject unit;
        if (!unitsByKey.TryGetValue(closetUnitID, out unit))
        {
            return null;
        }

        return unit.GetComponent<UnitController>();
    }

    public void NotifyAUnitKnockedOut(UnitType unitType, int unitID)
    {
        Dictionary<int, GameObject> unitsByKey = m_enemyUnits;
        if (unitType == UnitType.Player)
        {
            unitsByKey = m_playerUnits;
        }

        unitsByKey.Remove(unitID);

        if (m_playerUnits.Count == 0 || m_enemyUnits.Count == 0)
        {
            foreach (var item in m_playerUnits)
            {
                UnitController unitCtrler = item.Value.GetComponent<UnitController>();
                unitCtrler.OnVictory();
            }

            foreach (var item in m_enemyUnits)
            {
                UnitController unitCtrler = item.Value.GetComponent<UnitController>();
                unitCtrler.OnVictory();
            }

            UIManager.Instance.ShowUI("UIMenu");
        }
    }

    public void RecycleUnit(UnitType unitType, GameObject unit)
    {
        ObjectPooling.Instance.RecycleUnit(unitType, unit);
    }

    private void CleanBattleStage()
    {
        foreach (var item in m_playerUnits)
        {
            GameObject unit = item.Value;
            UnitController unitCtrler = unit.GetComponent<UnitController>();
            RecycleUnit(unitCtrler.UnitType, unit);
        }
        m_playerUnits.Clear();

        foreach (var item in m_enemyUnits)
        {
            GameObject unit = item.Value;
            UnitController unitCtrler = unit.GetComponent<UnitController>();
            RecycleUnit(unitCtrler.UnitType, unit);
        }
        m_enemyUnits.Clear();
    }
}

public enum GameMode
{
    OneVsOne = 1,
    OneVsMany = 2,
    ManyVsMany = 3,
}

public enum UnitType
{
    None = 0,
    Player = 1,
    Enemy = 2,
}
