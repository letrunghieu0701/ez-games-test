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
    private Transform m_playerSpawnPos;
    [SerializeField]
    private Transform m_enemySpawnPos;

    //[SerializeField]
    //private int m_numberOfEachTeam;

    [SerializeField]
    private GameMode m_gameMode = GameMode.OneVsMany;

    private Dictionary<int, GameObject> m_playerUnits = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> m_enemyUnits = new Dictionary<int, GameObject>();

    private UnityEvent<int> m_UnitKnockedOut = new UnityEvent<int>();
    private UnityEvent m_Victory = new UnityEvent();

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

    // Start is called before the first frame update
    void Start()
    {
        ObjectPooling.Instance.Init(m_initPlayerUnitPoolingNum, m_playerPrefab, m_initEnemyUnitPoolingNum, m_enemyPrefab);

        SetGameMode(m_gameMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameMode(GameMode gameMode)
    {
        if (gameMode == GameMode.OneVsOne)
        {
            GetUnit(UnitType.Player);
            GetUnit(UnitType.Enemy);
        }
        else if (gameMode == GameMode.OneVsMany)
        {
            GetUnit(UnitType.Player);

            for (int i = 0; i < 5; i++)
            {
                GetUnit(UnitType.Enemy);
            }
        }
        else if (gameMode == GameMode.ManyVsMany)
        {
            for (int i = 0; i < 5; i++)
            {
                GetUnit(UnitType.Player);
            }

            for (int i = 0; i < 5; i++)
            {
                GetUnit(UnitType.Enemy);
            }
        }
    }

    private void GetUnit(UnitType unitType)
    {
        Dictionary<int, GameObject> units = m_enemyUnits;
        Transform spawnPos = m_enemySpawnPos;
        if (unitType == UnitType.Player)
        {
            units = m_playerUnits;
            spawnPos = m_playerSpawnPos;
        }

        GameObject unit = ObjectPooling.Instance.GetUnit(unitType);
        unit.SetActive(true);
        Vector3 randomOffset = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f));
        unit.transform.position = spawnPos.position + randomOffset;

        HealthSystem unitHealthSys = unit.GetComponent<HealthSystem>();
        unitHealthSys.SetActiveHealhBar(true);
        BoxCollider bodyCollider = unit.GetComponent<BoxCollider>();
        bodyCollider.enabled = true;

        units.Add(unit.GetComponent<UnitController>().ID, unit);
        UnitController unitCtrler = unit.GetComponent<UnitController>();
        m_UnitKnockedOut.AddListener(unitCtrler.OnAUnitKnockedOut);
        m_Victory.AddListener(unitCtrler.OnVictory);
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

        GameObject unit = unitsByKey[unitID];
        UnitController unitCtrler = unit.GetComponent<UnitController>();
        m_UnitKnockedOut.RemoveListener(unitCtrler.OnAUnitKnockedOut);
        m_Victory.RemoveListener(unitCtrler.OnVictory);
        unitsByKey.Remove(unitID);

        m_UnitKnockedOut.Invoke(unitID);

        if (m_playerUnits.Count == 0 || m_enemyUnits.Count == 0)
        {
            m_Victory.Invoke();
        }
    }

    public void RecycleUnit(UnitType unitType, GameObject unit)
    {
        ObjectPooling.Instance.RecycleUnit(unitType, unit);
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
