using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class AudioUIManager : MonoBehaviour //, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float startTime = 0f;
    [SerializeField] private float currentTime;
    [SerializeField] private float Endtime;

    private void Awake()
    {
        EditorAudioManager.Instance.MusicInstance.getDescription(out EventDescription desc);
        desc.getLength(out int length);
        MakeGrid.Instance.gridCtrl_slider.value = length;
    }
}
