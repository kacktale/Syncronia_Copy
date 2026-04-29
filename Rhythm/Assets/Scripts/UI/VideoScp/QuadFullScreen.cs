using UnityEngine;
using UnityEngine.Video;

[DefaultExecutionOrder(-100)]
public class QuadFullScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RenderTexture targetRT; // 렌더 텍스처

    [SerializeField] private float videoZpos;

    void Awake()
    {
        FitToCamera();
    }

    private void Start()
    {
        SetupVideo();
    }

    private void SetupVideo()
    {
        if (SongSelectDataManager.Instance == null) return;

        var currentSong = SongSelectDataManager.Instance.CurrentSelectedSong;

        if (currentSong == null || currentSong.SongVideoClip == null)
        {
            Debug.LogWarning("[QuadFullScreen] 재생할 VideoClip이 없습니다. 배경을 비활성화합니다.");
            gameObject.SetActive(false); // 영상 없으면 Quad 비활성화
            return;
        }

        videoPlayer.clip = currentSong.SongVideoClip;

        // Render Texture 세팅
        if (targetRT != null)
        {
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = targetRT;
        }

        videoPlayer.Play();
        Debug.Log($"[QuadFullScreen] 영상 재생 시작 (길이: {videoPlayer.length})");
    }

    void FitToCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Main Camera 없음");
            return;
        }

        if (!cam.orthographic)
        {
            Debug.LogError("Orthographic 카메라 x");
            return;
        }

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        transform.localScale = new Vector3(width, height, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, videoZpos);
    }
}