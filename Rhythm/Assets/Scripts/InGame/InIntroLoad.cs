using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InIntroLoad : MonoBehaviour
{
    public static InIntroLoad Instance;

    public RectTransform loadingBar;
    public Image loadingImage;
    public Vector2[] posVec;

    [SerializeField] private FMODUnity.EventReference nextSceneBGM;

    private bool loadingOn = false;
    private float alpha = 1f;
    private bool isLoading = false;

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
            return;
        }
    }

    void Update()
    {
        if (loadingOn)
        {
            alpha = Mathf.Lerp(alpha, 1f, Time.smoothDeltaTime / 0.2f);
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, alpha);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[0], Time.smoothDeltaTime / 0.2f);
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 0f, Time.smoothDeltaTime / 0.2f);
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, alpha);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[1], Time.smoothDeltaTime / 0.2f);
        }
    }

    public void StartLoading(int sceneIndex)
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    private IEnumerator LoadSceneRoutine(int index)
    {
        loadingOn = true;

        FmodAudioManager.Instance.FadeOutBGM(0.8f);

        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene(index);

        EndLoading();

        if (!nextSceneBGM.IsNull)
            FmodAudioManager.Instance.FadeInBGM(nextSceneBGM, 0.8f);

        isLoading = false;
    }

    public void EndLoading()
    {
        loadingOn = false;
    }
}