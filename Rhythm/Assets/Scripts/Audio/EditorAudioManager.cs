using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class EditorAudioManager : MonoBehaviour
{
    public static EditorAudioManager Instance;

    [Header("Editor Music")]
    [SerializeField] private string currentSongPath = "None";

    public EventInstance MusicInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (FmodAudioManager.Instance != null)
        {
            FmodAudioManager.Instance.StopBGM();
            FmodAudioManager.Instance.gameObject.SetActive(false); 
        }
    }

    // 곡 교체 및 재생
    public void ChangeAndPlayMusic(string newEventPath)
    {
        if (MusicInstance.isValid())
        {
            MusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            MusicInstance.release(); // 메모리 릭 방지
        }

        currentSongPath = newEventPath;

        if (!string.IsNullOrEmpty(currentSongPath))
        {
            MusicInstance = RuntimeManager.CreateInstance(currentSongPath);
        }
    }

    // 재생 / 일시정지 토글
    public bool TogglePlayPause()
    {
        if (!MusicInstance.isValid()) return false;

        MusicInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            MusicInstance.start();
            return true;
        }
        else
        {
            MusicInstance.getPaused(out bool isPaused);
            MusicInstance.setPaused(!isPaused);
            return isPaused;
        }
    }

    // 완전 정지
    public void StopMusic()
    {
        if (MusicInstance.isValid())
        {
            // 즉시 정지 및 오디오 0초로 롤백
            MusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
            MusicInstance.setTimelinePosition(0);
        }
    }

    // 타임라인 점프
    public void SetTime(int milliseconds)
    {
        if (MusicInstance.isValid())
        {
            MusicInstance.setTimelinePosition(milliseconds);
        }
        // TODO : 나중에 그리드 클릭하면 그 시간대로 넘어가게 처리할 예정
    }

    // 현재 곡 진행 시간 가져오기
    public int GetCurrentTimeMS()
    {
        if (MusicInstance.isValid())
        {
            MusicInstance.getTimelinePosition(out int position);
            return position;
        }
        return 0;
    }
}