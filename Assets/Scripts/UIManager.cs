using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private static Dictionary<string, Transform> allUIs = new Dictionary<string, Transform>();

    private UIManager() { }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Init()
    {
        RegisterAllUI();
        HideAllUI();
    }

    public void RegisterAllUI()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            Transform ui = child.GetComponent<Transform>();
            if (ui == null)
            {
                continue;
            }
            allUIs[ui.name] = ui;
        }
    }

    public void HideAllUI()
    {
        foreach (Transform ui in allUIs.Values)
        {
            ui.gameObject.SetActive(false);
        }
    }

    public void ShowUI(string uiName)
    {
        if (allUIs.TryGetValue(uiName, out Transform ui))
        {
            ui.gameObject.SetActive(true);
        }
    }
}
