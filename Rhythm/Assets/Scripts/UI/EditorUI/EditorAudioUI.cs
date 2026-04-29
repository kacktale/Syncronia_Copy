using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EditorAudioUI : MonoBehaviour
{
    [Header("Button set")]
    [SerializeField] private Button play_btn;
    [SerializeField] private Image play_btn_img;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    [Header("NewInputSystem")]
    InputSystem_Actions inputActions;

    [Header("AudioPlay")]
    [SerializeField] private GameObject audioPlayblock;
    private bool wasPlaying = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.UI.AudioPlay.performed += ctx => OnClickAudioPause();
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }


    void Update()
    {
        // 매 프레임 위치를 동기화
        if (MakeGrid.Instance != null && MakeGrid.Instance.IsCreated)
        {
            EditorAudioManager.Instance.MusicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
            EditorAudioManager.Instance.MusicInstance.getPaused(out bool isPaused);

            bool isPlaying = (state == FMOD.Studio.PLAYBACK_STATE.PLAYING && !isPaused);

            //  슬라이더가 끝자락에 도달했거나 FMOD가 스스로 멈췄을 때
            float curVal = MakeGrid.Instance.gridCtrl_slider.value;
            float maxVal = MakeGrid.Instance.gridCtrl_slider.maxValue;

            // 근사값에 도달하면 멈추게 처리
            if (isPlaying && (curVal >= maxVal - 2.5f || state == FMOD.Studio.PLAYBACK_STATE.STOPPED))
            {
                EditorAudioManager.Instance.StopMusic();

                // 멈추는 순간 슬라이더를 곡의 실제 끝으로 보정
                MakeGrid.Instance.gridCtrl_slider.value = maxVal;

                wasPlaying = false;
                UpdateButtonIcon(false); 

                Debug.Log("곡 재생이 실제 끝 지점에 도달하여 정지합니다.");
                return;
            }

            if (isPlaying)
            {
                GridSliderAudio(); // 재생 중엔 자연스럽게 float 이동
                wasPlaying = true;
            }
            else if (wasPlaying) 
            {
                // 방금 전까지 재생 중이다가 멈춘 순간, 가장 가까운 정수 비트로 스냅
                MakeGrid.Instance.gridCtrl_slider.value = Mathf.RoundToInt(MakeGrid.Instance.gridCtrl_slider.value);
                wasPlaying = false;
            }
        }
    }
    void Start()
    {
        if (MakeGrid.Instance != null)
        {
            MakeGrid.Instance.OnGridCreated += EnablePlayButton;
            play_btn.interactable = MakeGrid.Instance.IsCreated;
        }

        if (play_btn != null)
        {
            play_btn.onClick.AddListener(OnClickAudioPause);
        }

        audioPlayblock.SetActive(false);
    }

    private void OnDestroy()
    {
        if (MakeGrid.Instance != null)
        {
            MakeGrid.Instance.OnGridCreated -= EnablePlayButton;
        }
    }

    void OnClickAudioPause()
    {
        if (EditorAudioManager.Instance == null)
        {
            Debug.LogWarning("������ ����� �Ŵ��� ����");
            return;
        }

        bool isPlaying = EditorAudioManager.Instance.TogglePlayPause();

        UpdateButtonIcon(isPlaying);
    }

    private void UpdateButtonIcon(bool isPlaying)
    {
        if (play_btn_img != null)
        {
            if(isPlaying)
            {
                play_btn_img.sprite = pauseIcon;
                audioPlayblock.SetActive(true);
            }
            else
            {
                play_btn_img.sprite = playIcon;
                audioPlayblock.SetActive(false);
            }
        }
    }

    public void ResetUI()
    {
        UpdateButtonIcon(false);
    }

    private void EnablePlayButton()
    {
        play_btn.interactable = true;
    }

    private void GridSliderAudio()
    {
        if (MakeGrid.Instance == null || !MakeGrid.Instance.IsCreated || EditorAudioManager.Instance == null) return;

        // 현재 ms -> 비트(Slider Value) 변환 로직
        int currentMs = EditorAudioManager.Instance.GetCurrentTimeMS();
        float bpm = EditorSave.instance.data.BPM;
        
        if (bpm <= 0) return;

        float msPerBeat = 60000f / bpm;
        float sliderAudioValue = currentMs / msPerBeat;

        // 슬라이더 값 갱신
        MakeGrid.Instance.gridCtrl_slider.value = sliderAudioValue;
    }
}
