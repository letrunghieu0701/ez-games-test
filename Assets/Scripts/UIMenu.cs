using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    private Button m_BtnStartGame;
    private TMP_Dropdown m_dropdownLevel;

    private void Awake()
    {
        m_BtnStartGame = this.transform.Find("Canvas/BtnStartGame").GetComponent<Button>();
        m_dropdownLevel = this.transform.Find("Canvas/DropdownLevel").GetComponent<TMP_Dropdown>();
    }

    void Start()
    {
        m_BtnStartGame.onClick.AddListener(OnClickBtnStartGame);

        List<LevelData> levelDatas = GameManager.Instance.m_levelDataSO.Levels;
        for (int i = 0; i < levelDatas.Count; i++)
        {
            m_dropdownLevel.options.Add(new TMP_Dropdown.OptionData(levelDatas[i].Name));
        }

        m_dropdownLevel.value = 0;
        m_dropdownLevel.RefreshShownValue();
    }

    private void OnClickBtnStartGame()
    {
        if (0 <= m_dropdownLevel.value && m_dropdownLevel.value < 10)
        {
            GameManager.Instance.LoadGameMode(m_dropdownLevel.value + 1);
        }
    }

    void Update()
    {
        
    }
}
