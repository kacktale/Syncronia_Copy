using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class InputValidator : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private int minCharacterLimit = 1;

    private void OnEnable()
    {
        inputField.onValidateInput += ValidateChar;
        inputField.onValueChanged.AddListener(RemoveInvalidChars);
    }

    private void OnDisable()
    {
        inputField.onValidateInput -= ValidateChar;
        inputField.onValueChanged.RemoveListener(RemoveInvalidChars);
    }

    private char ValidateChar(string text, int index, char addedChar)
    {
        if (char.IsLetterOrDigit(addedChar) || addedChar == '_') return addedChar;
        return '\0';
    }

    private void RemoveInvalidChars(string text)
    {
        string cleanText = Regex.Replace(text, @"[^a-zA-Z0-9_]", "");

        if (cleanText != text)
        {
            inputField.text = cleanText;
            inputField.caretPosition = cleanText.Length;
        }

        inputField.textComponent.color = IsValid() ? Color.black : Color.red;

        if (cleanText != text) Debug.Log($"[InputValidator] Invalid characters removed. Current: {cleanText}");
    }

    public bool IsValid() => inputField.text.Length >= minCharacterLimit;

    public string GetText() => inputField.text;
}