using System.Collections;
using System.IO; // ���� üũ��
using TMPro;
using UnityEngine;

public class SaveEditorPanel : PanelBase
{
    public override PanelType PanelType => PanelType.Editor;

    [Header("UI References")]
    [SerializeField] private GameObject saveEditorPanel;
    [SerializeField] private GameObject blind_img;
    [SerializeField] private InputValidator save_inputValidator;
    [SerializeField] private TMP_Text status_txt;

    [Header("System")]
    [SerializeField] private EditorSave editorSave;

    protected override void Awake()
    {
        base.Awake();
        if (status_txt != null) status_txt.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        if (status_txt != null) status_txt.gameObject.SetActive(false);
    }

    protected override void OnHide()
    {

    }

    public void OnClickSave()
    {
        if (!save_inputValidator.IsValid())
        {
            StartCoroutine(ShowMessage("Name is too short!", Color.red));
            return;
        }

        string fileName = save_inputValidator.GetText();

        // ���� �̸� ��ø�Ǵ��� üũ�ϴ� �κ�
        if (editorSave.CheckFileExist(fileName))
        {
            StartCoroutine(ShowMessage("File Already Exists!", Color.red));
            return;
        }

        editorSave.SaveData(fileName);
        StartCoroutine(ShowMessage("Save Complete!", Color.green));
    }

    IEnumerator ShowMessage(string msg, Color color)
    {
        status_txt.text = msg;
        status_txt.color = color;
        status_txt.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        float duration = 1.0f;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            status_txt.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        status_txt.gameObject.SetActive(false);
    }
}