using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IntroSceneSelect : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex = 2;
    [SerializeField] private Button startButton;

    private bool selected = false;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnClickStart);
    }

    void Update()
    {
        if (selected) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    public void OnClickStart()
    {
        if (selected) return;
        StartGame();
    }

    private void StartGame()
    {
        selected = true;
        InIntroLoad.Instance.StartLoading(targetSceneIndex);
    }
}