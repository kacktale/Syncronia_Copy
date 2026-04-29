using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class FmodAudioManager : MonoBehaviour
{
    public static FmodAudioManager Instance;

    [Header("BGM")]
    public EventReference bgmEvent;
    private EventInstance bgmInstance;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // 시작 시 BGM 재생
        if (!bgmEvent.IsNull)
        {
            PlayInitialBGM();
        }
    }

    private void PlayInitialBGM()
    {
        bgmInstance = RuntimeManager.CreateInstance(bgmEvent);
        bgmInstance.start();
        bgmInstance.release();
    }

    public void StopBGM(bool immediate = false)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(immediate ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void FadeOutBGM(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.getVolume(out float startVolume);
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                bgmInstance.setVolume(Mathf.Lerp(startVolume, 0f, timer / duration));
                yield return null;
            }
            StopBGM(true);
        }
    }

    public void FadeInBGM(EventReference nextBgm, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(nextBgm, duration));
    }

    private IEnumerator FadeInCoroutine(EventReference nextBgm, float duration)
    {
        StopBGM(true);

        // 새 곡 생성 및 재생
        bgmInstance = RuntimeManager.CreateInstance(nextBgm);
        bgmInstance.setVolume(0f);
        bgmInstance.start();
        bgmInstance.release();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            bgmInstance.setVolume(Mathf.Lerp(0f, 1f, timer / duration));
            yield return null;
        }
    }

    public int GetEventLength(EventReference reference)
    {
        if (reference.IsNull) return 0;
        var description = RuntimeManager.GetEventDescription(reference);
        if (description.isValid())
        {
            description.getLength(out int length);
            return length;
        }
        return 0;
    }

    public void PlaySFX(EventReference sfx)
    {
        if (!sfx.IsNull) RuntimeManager.PlayOneShot(sfx);
    }
}