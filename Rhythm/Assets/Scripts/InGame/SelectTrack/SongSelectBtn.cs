using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectBtn : MonoBehaviour
{
    [Header("UI")]
    //[SerializeField] private TextMeshProUGUI titleText;
    //[SerializeField] private TextMeshProUGUI composer;
    [SerializeField] private Image titleBtn_img;

    [SerializeField] private SongData mySongData;

    public TrackInput trackInput;

    public void Setup(SongData songdata)
    {
        if (songdata == null) return;
        //titleText.text = songdata.title;
        //composer.text = songdata.composer;
        titleBtn_img.sprite = songdata.title_img;
    }

    public void OnClickButton()
    {
        if (trackInput != null)
        {
            trackInput.StartGameWithCurrentSelection();
        }
    }
}