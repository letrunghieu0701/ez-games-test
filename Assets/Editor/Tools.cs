using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class Tools
{
    [MenuItem("MyTools/Generate Levels")]
    public static void GenerateLevels()
    {
        LevelDataSO asset = ScriptableObject.CreateInstance<LevelDataSO>();

        string folderPath = "Assets/ScriptableObjects";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        // Scale
        int initialOneVsManyNum = 1;
        int increamentOneVsManyNum = 1;

        int initialManyVsManyNum = 2;
        int increamentManyVsManyNum = 3;

        float initialScale = 0.3f;
        float increamentScale = 0.5f;

        // Write config
        asset.Levels = new List<LevelData>();
        for (int i = 0; i < 10; i++)
        {
            LevelData data = new LevelData();
            data.ID = i + 1;
            data.Name = "Level " + (i + 1);
            data.GameMode = (GameMode)((i % 3) + 1);

            float playerScale = initialScale + increamentScale * i;

            if (data.GameMode == GameMode.OneVsOne)
            {
                data.PlayerUnitNum = 1;
                data.EnemyUnitNum = 1;
            }
            else if (data.GameMode == GameMode.OneVsMany)
            {
                data.PlayerUnitNum = 1;
                data.EnemyUnitNum = initialOneVsManyNum + increamentOneVsManyNum;
                increamentOneVsManyNum += 1;
                playerScale *= 1.75f;
            }
            else if (data.GameMode == GameMode.ManyVsMany)
            {
                data.PlayerUnitNum = initialManyVsManyNum + increamentManyVsManyNum * i;
                data.EnemyUnitNum = data.PlayerUnitNum;
            }

            data.ScaleAttackPlayer = playerScale;
            data.ScaleHealthPlayer = playerScale;
            data.ScaleMoveSpeedPlayer = Mathf.Clamp(playerScale, 1, 1.2f);

            float enemyScale = initialScale + increamentScale * i;
            data.ScaleAttackEnemy = enemyScale;
            data.ScaleHealthEnemy = enemyScale;
            data.ScaleMoveSpeedEnemy = Mathf.Clamp(enemyScale, 1, 1.2f);

            asset.Levels.Add(data);
        }

        string filePath = $"{folderPath}/LevelData.asset";
        AssetDatabase.CreateAsset(asset, filePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Generated Levels: Completed");
    }
}
