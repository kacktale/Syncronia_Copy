using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSpeed : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedValue_Text;
    [SerializeField] private Button speedPlus_Btn;
    [SerializeField] private Button speedMinus_Btn;

    private void Awake()
    {
        speedValue_Text.text = UserData.instance.userSpeed.ToString("F1") + "X";
        speedPlus_Btn.onClick.AddListener(() => ChangeSpeed(0.5f));
        speedMinus_Btn.onClick.AddListener(() => ChangeSpeed(-0.5f));
    }

    private void ChangeSpeed(float value)
    {
        float speed = Mathf.Clamp(UserData.instance.userSpeed + value, 3, 10);
        UserData.instance.userSpeed = speed;

        speedValue_Text.text = UserData.instance.userSpeed.ToString("F1") + "X";
    }
}
