using System.Collections;
using TMPro;
using UnityEngine;

public class LoadEditorPanel : PanelBase
{
    public override PanelType PanelType => PanelType.Editor;


    [Header("UI")]
    [SerializeField] private GameObject loadEditorPanel;
    [SerializeField] private GameObject blind_img;
    [SerializeField] private InputValidator load_inputValidator;
    [SerializeField] private TMP_Text statusText;

    [Header("System")]
    [SerializeField] private EditorSave editorSave;

    protected override void Awake()
    {
        base.Awake();
        if (statusText != null) statusText.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        if (statusText != null) statusText.gameObject.SetActive(false);
    }

    protected override void OnHide()
    {

    }

    public void OnClickLoad()
    {
        if (!load_inputValidator.IsValid())
        {
            StartCoroutine(ShowMessage("Enter File Name!", Color.red));
            return;
        }

        string fileName = load_inputValidator.GetText();

        if (editorSave.CheckFileExist(fileName))
        {
            editorSave.LoadData(fileName);
            StartCoroutine(ShowMessage("Load Complete!", Color.green));
            MakeGrid.Instance.LoadGridMap();
            MakeNote.Instance.NoteInstallLoad();
        }
        else
        {
            StartCoroutine(ShowMessage("File Not Found!", Color.red));
        }
    }

    IEnumerator ShowMessage(string msg, Color color)
    {
        statusText.text = msg;
        statusText.color = color;
        statusText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        float duration = 1.0f;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            statusText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        statusText.gameObject.SetActive(false);
    }
}