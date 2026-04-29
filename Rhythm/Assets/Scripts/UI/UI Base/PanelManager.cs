using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    private Dictionary<Type, PanelBase> registeredPanels = new Dictionary<Type, PanelBase>();

    public bool IsBlockPanelOpen { get; private set; }
    public bool IsEditorPanelOpen { get; private set; }
    public int ActivePopupCount { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPanelInstance(PanelBase panel)
    {
        Type type = panel.GetType();
        if (!registeredPanels.ContainsKey(type))
        {
            registeredPanels.Add(type, panel);
        }
    }

    // ���� ��û
    public void ShowPanel<T>() where T : PanelBase
    {
        if (registeredPanels.TryGetValue(typeof(T), out PanelBase panel))
        {
            panel.Show();
        }
        else Debug.LogWarning($"[PanelManager] {typeof(T).Name} �г��� ��� X");
    }

    // �ݱ� ��û
    public void HidePanel<T>() where T : PanelBase
    {
        if (registeredPanels.TryGetValue(typeof(T), out PanelBase panel))
        {
            panel.Hide();
        }
    }

    public void HideAllPanel<T>() where T : PanelBase
    {
        foreach (var panel in registeredPanels.Values)
        {
            if (panel.PanelOpen())
            {
                panel.Hide();
            }
        }
    }

    // type�� ���� ���� ����
    public bool CanOpenPanel(PanelType type)
    {
        if (type == PanelType.Hud) return true;
        
        if (IsBlockPanelOpen || IsEditorPanelOpen) return false;

        return true;
    }

    public void RegisterPanelOpen(PanelType type)
    {
        if (type == PanelType.Block) IsBlockPanelOpen = true;
        else if (type == PanelType.Popup) ActivePopupCount++;
        else if (type == PanelType.Editor)
        {
            IsEditorPanelOpen = true;
        }
    }

    public void RegisterPanelClose(PanelType type)
    {
        if (type == PanelType.Block) IsBlockPanelOpen = false;
        else if (type == PanelType.Popup) ActivePopupCount = Mathf.Max(0, ActivePopupCount - 1);
        else if (type == PanelType.Editor)
        {
            IsEditorPanelOpen = false;
        }
    }
}