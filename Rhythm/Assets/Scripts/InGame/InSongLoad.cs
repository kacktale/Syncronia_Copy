using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InSongLoad : MonoBehaviour
{
    public static InSongLoad Instance;

    [Header("UI Wrapper")]
    [SerializeField] private GameObject songLoading_obj;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("LOAD UI")]
    [SerializeField] private RectTransform loadingBar;
    [SerializeField] private Image loading_img;
    [SerializeField] private Image loadSong_img;
    [SerializeField] private TextMeshProUGUI loadSong_txt;

    public Vector2[] posVec;

    private bool loadingOn = false;
    private float alpha = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        loading_img.gameObject.SetActive(false);
        EndLoading();
    }

    void Update()
    {
        if (loadingOn)
        {
            if (!songLoading_obj.activeSelf) songLoading_obj.SetActive(true);

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * 5f);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[0], Time.deltaTime * 5f);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * 5f);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[1], Time.deltaTime * 5f);

            if (canvasGroup.alpha <= 0.001f && songLoading_obj.activeSelf)
            {
                songLoading_obj.SetActive(false);
            }
        }
    }

    public void StartSongLoading(int sceneIndex, string songName, Sprite songImage, FMODUnity.EventReference fmodfile)
    {
        Debug.Log($"[InSongLoad] Load Index: {sceneIndex} / Song : {songName}");
        FmodAudioManager.Instance.FadeOutBGM(0.5f);

        canvasGroup.alpha = 0f;
        songLoading_obj.SetActive(true);

        loadSong_txt.text = songName;
        loadSong_img.sprite = songImage;

        StartCoroutine(LoadSceneRoutine(sceneIndex, string.Empty, fmodfile));
    }

    public void StartSongLoading(string sceneName, string songName, Sprite songImage, FMODUnity.EventReference fmodfile)
    {
        Debug.Log($"[InSongLoad] Load Name: {sceneName} / Song: {songName}");
        FmodAudioManager.Instance.FadeOutBGM(0.5f);

        canvasGroup.alpha = 0f;
        songLoading_obj.SetActive(true);

        loadSong_txt.text = songName;
        loadSong_img.sprite = songImage;

        // Path ´ë½Å EventReference °´Ã¼ ÀÚÃ¼¸¦ Àü´Þ
        StartCoroutine(LoadSceneRoutine(-1, sceneName, fmodfile));
    }

    private IEnumerator LoadSceneRoutine(int index, string name, FMODUnity.EventReference nextFmod)
    {
        loadingOn = true;

        yield return new WaitForSecondsRealtime(2f);

        if (!nextFmod.IsNull) FmodAudioManager.Instance.FadeInBGM(nextFmod, 0.5f);

        if (!string.IsNullOrEmpty(name)) SceneManager.LoadScene(name);
        else if (index >= 0) SceneManager.LoadScene(index);
        else Debug.LogError("[InSongLoad] ¾ÀÀÌ ¾ø½¹");

        EndLoading();
    }

    public void EndLoading() => loadingOn = false;
}