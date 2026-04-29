using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelect : MonoBehaviour
{
    public Button shortNote;
    public Button longNote;
    public Button speedNote;
    public RectTransform selectedTransform;
    public Vector3[] setPos;
    public TMP_Dropdown bitDropdown;

    private MakeNote makeNote;
    public void Start()
    {
        makeNote = GetComponent<MakeNote>();
        shortNote.onClick.AddListener(delegate { SetDirection(0); });
        longNote.onClick.AddListener(delegate { SetDirection(1); });
        speedNote.onClick.AddListener(delegate { SetDirection(3); });
        bitDropdown.onValueChanged.AddListener(delegate { SetBit(bitDropdown); });
    }
    void SetDirection(int dir)
    {
        
        selectedTransform.localPosition = dir != 3 ? new Vector3(setPos[dir].x, selectedTransform.localPosition.y, 0) : new Vector3(setPos[dir - 1].x, selectedTransform.localPosition.y, 0);
        makeNote.selectNoteType = dir;
        makeNote.ChangePreNote();
    }

    public void SetBit(TMP_Dropdown select)
    {
        makeNote.BeatMode = new bool[3];
        makeNote.BeatMode[select.value] = true;
        makeNote.beatInput = select.value;
    }
}
