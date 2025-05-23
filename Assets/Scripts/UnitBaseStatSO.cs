using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitBaseStat", menuName = "Unit Base Stat Data")]
public class UnitBaseStatDataSO : ScriptableObject
{
    public List<UnitBaseStatData> Stats;
}

[System.Serializable]
public class UnitBaseStatData
{
    public int ID;
    public string Name;
    public UnitType UnitType;
    public int Attack;
    public int Health;
    public float MoveSpeed;
}

