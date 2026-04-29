using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public EventReference MusicReference; //Audio Sources
    public EventInstance MusicInstance; // Audio Player

    [Header("SFX")]
    public EventReference SFXReference;
    public EventInstance SFXInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);

        MusicInstance = RuntimeManager.CreateInstance(MusicReference);
    }
    private void Start()
    {
        //StartMusic();
    }

    void Update()
    {

    }

    void StartMusic()
    {
        MusicInstance.start();
    }

    public void ChangeMusic(string newEventPath)
    {
        // ���� ���� ��� ���̶�� ���� �� �� �� ���
        MusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MusicInstance.release();

        // �� ��η� �� ���
        MusicInstance = RuntimeManager.CreateInstance(newEventPath);

    }
}