using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SceneSelect : MonoBehaviour
{
    [Header("Scene Info")]
    public int selectedSceneNum = 1;
    public string[] sceneNames;
    [SerializeField] private TMP_Text targetName_txt;
    [SerializeField] private float sceneTextSpeed = 10f;
    private Vector3 textOriginalScale;

    public RectTransform selectedTransform;
    public Vector2[] sceneImagePos;

    private bool selected = false;

    void Start()
    {
        textOriginalScale = targetName_txt.rectTransform.localScale;
        UpdateSceneNameDisplay();
    }

    void Update()
    {
        selectedTransform.anchoredPosition = Vector3.Lerp(selectedTransform.anchoredPosition, sceneImagePos[selectedSceneNum], Time.deltaTime / 0.1f);
        targetName_txt.rectTransform.localScale = Vector3.Lerp(targetName_txt.rectTransform.localScale, textOriginalScale, Time.deltaTime * sceneTextSpeed);
    }

    public void OnNavigate(InputValue context)
    {
        Vector2 contPos = context.Get<Vector2>();
        if (contPos == Vector2.zero || selected) return;

        selectedSceneNum = (selectedSceneNum + sceneNames.Length + (int)contPos.x) % sceneNames.Length;

        targetName_txt.rectTransform.localScale = textOriginalScale * 0.6f;
        UpdateSceneNameDisplay();
    }

    public void OnConfirmd(InputValue context)
    {
        if (!context.isPressed || selected) return;
        selected = true;
        InGameLoad.Instance.StartLoading(selectedSceneNum + 2);
    }

    private void UpdateSceneNameDisplay()
    {
        if (sceneNames.Length > selectedSceneNum)
        {
            targetName_txt.text = sceneNames[selectedSceneNum];
            targetName_txt.rectTransform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }
    }
}