using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System;

public class MakeGrid : MonoBehaviour
{
    public static MakeGrid Instance;
    public GameObject gridLine;
    public event Action OnGridCreated;
    public bool IsCreated = false;

    public float gridGapForce;
    public Vector3 gridPos;
    private Vector3 initialGridPos; // 로드하기 전에 원래 위치 저장하는 용도로 쓰는 Pos

    public Transform metronomeParant;
    public Button makeButton;
    public TMP_InputField bpmInput;
    public EditorSave editorSave;

    [Header("Grid UI")]

    public Slider gridCtrl_slider;
    private float lastSliderValue = 0f;
    // 시작할 때의 원본 위치를 기억해둘 변수
    private Vector3 baseMetronomePos; 
    private Vector3 baseNotePos;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        makeButton.onClick.AddListener(MakeGridMap);

        initialGridPos = gridPos;

        baseMetronomePos = metronomeParant.transform.position;

        if (gridCtrl_slider != null)
        {
            gridCtrl_slider.wholeNumbers = false; 
            gridCtrl_slider.onValueChanged.AddListener(OnGridCtrlSliderMoved);
        }
    }


    void Update()
    {
        if(Input.mouseScrollDelta.y != 0 && IsCreated && gridCtrl_slider != null)
        {
            gridCtrl_slider.value += Input.mouseScrollDelta.y;
        }
    }

    void OnGridCtrlSliderMoved(float value)
    {
        // MakeNote가 준비되었을 때 최초 1회 위치 저장 (위치 갈리는 거 방지용으로 만듬)
        if (baseNotePos == Vector3.zero && MakeNote.Instance != null && MakeNote.Instance.noteParants != null)
        {
            baseNotePos = MakeNote.Instance.noteParants.transform.position;
        }

        // 오디오 상태 및 일시정지 여부 확인
        EditorAudioManager.Instance.MusicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        EditorAudioManager.Instance.MusicInstance.getPaused(out bool isPaused);

        // 재생 중이 아닐 때(정지/일시정지) 혹은 직접 조작 시 정수 스냅 처리
        bool isPlaying = (state == FMOD.Studio.PLAYBACK_STATE.PLAYING && !isPaused);

        if (!isPlaying)
        {
            int roundedValue = Mathf.RoundToInt(value);

            // 소수점 값이 들어오면 강제로 정수로 맞춤
            if (Mathf.Abs(value - roundedValue) > 0.001f)
            {
                gridCtrl_slider.value = roundedValue;
                return;
            }

            // 정수 값 기준으로 위치 계산
            float yOffset = roundedValue * gridGapForce;

            // 그리드 및 노트 부모 오브젝트 위치 업데이트
            metronomeParant.transform.position = new Vector3(baseMetronomePos.x, baseMetronomePos.y - yOffset, baseMetronomePos.z);
            if (MakeNote.Instance != null && MakeNote.Instance.noteParants != null)
            {
                MakeNote.Instance.noteParants.transform.position = new Vector3(baseNotePos.x, baseNotePos.y - yOffset, baseNotePos.z);
            }

            // 오디오 타임라인 동기화 (재생 중이 아닐 때만)
            float bpm = EditorSave.instance.data.BPM;
            if (bpm > 0)
            {
                float msPerBeat = 60000f / bpm;
                int targetTimeMs = Mathf.RoundToInt(roundedValue * msPerBeat);
                EditorAudioManager.Instance.SetTime(targetTimeMs);
            }
        }
        else
        {
            // 재생 중에는 소수점 단위로 부드럽게 업데이트
            float yOffset = value * gridGapForce;

            metronomeParant.transform.position = new Vector3(baseMetronomePos.x, baseMetronomePos.y - yOffset, baseMetronomePos.z);
            if (MakeNote.Instance != null && MakeNote.Instance.noteParants != null)
            {
                MakeNote.Instance.noteParants.transform.position = new Vector3(baseNotePos.x, baseNotePos.y - yOffset, baseNotePos.z);
            }
        }
    }

    IEnumerator SetMetronome()
    {
        PanelManager.Instance.ShowPanel<LoadingPanel>();
        EditorAudioManager.Instance.MusicInstance.getDescription(out EventDescription desc);
        desc.getLength(out int length);
        float songTime = length / 1000f;
        //float songTime = FmodAudioManager.Instance.GetSongLength();
        Debug.Log("곡 시간 : " + songTime);

        float beatInterval = 60f / float.Parse(bpmInput.text);
        float currentInterval = 0.0f;

        int spawnedMetronome = 1;
        while (currentInterval < songTime)
        {
            Instantiate(gridLine, gridPos, Quaternion.identity, metronomeParant);
            gridPos.y += gridGapForce;
            currentInterval += beatInterval;
            spawnedMetronome++;
        }
        yield return null;

        if (gridCtrl_slider != null)
        {
            gridCtrl_slider.minValue = 0;
            // 생성된 메트로놈 개수를 최대값으로 설정
            gridCtrl_slider.maxValue = spawnedMetronome; 
            gridCtrl_slider.value = 0;
            lastSliderValue = 0;
        }

        EditorSave.instance.data.metronomeCount = spawnedMetronome;
        EditorSave.instance.data.BPM = int.Parse(bpmInput.text);
        Debug.Log(spawnedMetronome + ": 개가 생성됨");
        PanelManager.Instance.HidePanel<LoadingPanel>();
        IsCreated = true;

        OnGridCreated?.Invoke();
    }

    IEnumerator LoadSetMetronome()
    {
        PanelManager.Instance.ShowPanel<LoadingPanel>();
        EditorAudioManager.Instance.MusicInstance.getDescription(out EventDescription desc);
        desc.getLength(out int length);
        float songTime = length / 1000f;
        //float songTime = FmodAudioManager.Instance.GetSongLength();
        Debug.Log("곡 시간 : " + songTime);

        float beatInterval = 60f / editorSave.data.BPM;
        float currentInterval = 0.0f;

        int spawnedMetronome = 1;
        while (currentInterval < songTime)
        {
            Instantiate(gridLine, gridPos, Quaternion.identity, metronomeParant);
            gridPos.y += gridGapForce;
            currentInterval += beatInterval;
            spawnedMetronome++;
        }
        yield return null;

        if (gridCtrl_slider != null)
        {
            gridCtrl_slider.minValue = 0;
            gridCtrl_slider.maxValue = spawnedMetronome; 
            gridCtrl_slider.value = 0;
            lastSliderValue = 0;
        }

        EditorSave.instance.data.metronomeCount = spawnedMetronome;
        EditorSave.instance.data.BPM = editorSave.data.BPM;
        Debug.Log(spawnedMetronome + ": 개가 생성됨");
        bpmInput.text = editorSave.data.BPM.ToString();
        PanelManager.Instance.HidePanel<LoadingPanel>();
        IsCreated = true;

        OnGridCreated?.Invoke();
    }

    public void ClearGrid()
    {
        IsCreated = false;

        gridPos = initialGridPos;

        foreach (Transform child in metronomeParant)
        {
            Destroy(child.gameObject);
        }

        if (gridCtrl_slider != null)
        {
            gridCtrl_slider.value = 0;
        }
    }

    public void MakeGridMap()
    {
        ClearGrid();

        if (!IsCreated)
        {
            StartCoroutine(SetMetronome());
        }
    }

    public void LoadGridMap()
    {
        ClearGrid();

        if (!IsCreated)
        {
            StartCoroutine(LoadSetMetronome());
        }
    }
}
