using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    public abstract PanelType PanelType { get; }
    public bool panelOpen { get; private set; }
    [SerializeField] protected bool startOpen = false; // 처음에 열고 시작할 건지 아닌지 선택하게 만드러써요 <- 좋은데요?

    protected virtual void Awake()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.RegisterPanelInstance(this);
        }

        if (!startOpen)
        {
            gameObject.SetActive(false);
            panelOpen = false;
        }
        else
        {
            panelOpen = true;
            OnShow();
        }
    }

    public virtual void Show()
    {
        if (panelOpen) return;

        // 매니저 룰 검사
        if (PanelManager.Instance != null && !PanelManager.Instance.CanOpenPanel(PanelType))
        {
            Debug.LogWarning($"[PanelBase] {gameObject.name} 열기 거부됨: 상위 우선순위 패널 존재");
            return;
        }

        panelOpen = true;
        if (PanelManager.Instance != null)
            PanelManager.Instance.RegisterPanelOpen(PanelType);

        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide()
    {
        if (!panelOpen) return;

        panelOpen = false;
        if (PanelManager.Instance != null)
            PanelManager.Instance.RegisterPanelClose(PanelType);

        OnHide();
        gameObject.SetActive(false);
    }

    public bool PanelOpen() => panelOpen;

    protected abstract void OnShow();
    protected abstract void OnHide();
}