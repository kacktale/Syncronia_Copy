using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : PanelBase
{
    public override PanelType PanelType => PanelType.Block;

    [SerializeField] private GameObject gamePausePanel;

    [Header("Game Pause Btn")]
    [SerializeField] private Button continue_btn;
    [SerializeField] private Button reStart_btn;
    [SerializeField] private Button exit_btn;

    protected override void Awake()
    {
        base.Awake();
        if (continue_btn != null) continue_btn.onClick.AddListener(GameContinue);
        if (reStart_btn != null) reStart_btn.onClick.AddListener(GameReStart);
        if (exit_btn != null) exit_btn.onClick.AddListener(GameExit);
    }

    protected override void OnShow()
    {
        gamePausePanel.SetActive(true);
    }

    protected override void OnHide()
    {
        gamePausePanel.SetActive(false);
    }

    void Update()
    {
        
    }

    void GameContinue()
    {
        CreateMetronome.Instance.PauseCall();
        TrackData.Instance.StartTrack();
    }

    void GameReStart()
    {

    }

    void GameExit()
    {

    }
}
