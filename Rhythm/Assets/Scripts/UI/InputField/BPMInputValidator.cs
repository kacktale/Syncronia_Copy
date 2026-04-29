using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BPMInputValidator : MonoBehaviour
{
    [SerializeField] private TMP_InputField bpmInput;
    [SerializeField] private Button bpmBtn;

    void Start()
    {
        bpmInput.characterLimit = 3;

        bpmInput.onValueChanged.AddListener(CheckBPM);
        CheckBPM(bpmInput.text);
    }

    void CheckBPM(string text)
    {
        bool isValid = text.Length > 0 && int.TryParse(text, out int val) && val > 0;

        if (isValid)
        {
            bpmInput.textComponent.color = Color.black;
            bpmBtn.interactable = true;
        }
        else
        {
            bpmInput.textComponent.color = Color.red;
            bpmBtn.interactable = false;
        }
    }
}
