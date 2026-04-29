using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditCommendHUD : MonoBehaviour
{
    public static EditCommendHUD Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject redoHUD;
    [SerializeField] private GameObject undoHUD;

    private float disappearTime = 0.8f;
    private Coroutine fadeRoutine;

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
    }

    private void Start()
    {
        redoHUD.SetActive(false);
        undoHUD.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    public void TriggerRedo() => StartFadeEffect(redoHUD, undoHUD);
    public void TriggerUndo() => StartFadeEffect(undoHUD, redoHUD);

    private void StartFadeEffect(GameObject showObj, GameObject hideObj)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutRoutine(showObj, hideObj));
    }

    private IEnumerator FadeOutRoutine(GameObject target, GameObject other)
    {
        other.SetActive(false);
        target.SetActive(true);
        canvasGroup.alpha = 1.0f;

        float timer = 0f;
        while (timer < disappearTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - (timer / disappearTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        target.SetActive(false);
        fadeRoutine = null;
    }
}