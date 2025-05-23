using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    public List<LevelData> Levels;
}

[System.Serializable]
public class LevelData
{
    public int ID;
    public string Name;
    public GameMode GameMode;
    public int PlayerUnitNum;
    public int EnemyUnitNum;
    public float ScaleAttackPlayer;
    public float ScaleHealthPlayer;
    public float ScaleMoveSpeedPlayer;
    public float ScaleAttackEnemy;
    public float ScaleHealthEnemy;
    public float ScaleMoveSpeedEnemy;
}
