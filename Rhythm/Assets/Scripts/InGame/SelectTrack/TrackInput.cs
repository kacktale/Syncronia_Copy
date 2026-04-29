using TrackSelect;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TrackInput : MonoBehaviour
{
    public DifficultSelect difficultSelect;
    public SongSelect songSelect;
    public SongCarouselManager carouselManager;

    private int currentDiffListIndex = 0;

    private void Start()
    {
        // 시작 시 데이터가 존재하면 첫 곡으로 캐러셀 및 UI 초기화
        if (carouselManager != null && carouselManager.ReadOnlySongList.Count > 0)
        {
            Invoke(nameof(InitUI), 0.1f);
        }
    }

    private void InitUI()
    {
        songSelect.InitCarousel(0);
        ChangeSongUI(0);
    }

    private void ChangeSongUI(int songIndex)
    {
        var currentSong = carouselManager.ReadOnlySongList[songIndex];

        FmodAudioManager.Instance.FadeInBGM(currentSong.FmodEvent, 0.5f);

        //난이도 선택 초기화
        currentDiffListIndex = 0;

        if (currentSong.difficulties != null && currentSong.difficulties.Count > 0)
        {
            // Enum 기반으로 첫 번째 난이도의 UI 위치 설정
            difficultSelect.difficult = (int)currentSong.difficulties[0].difficultyType;
        }

        SongDetailInfo.Instance.UpdateSongInfo(currentSong);
    }

    public void OnSubmit(InputValue context)
    {
        StartGameWithCurrentSelection();
    }

    public void StartGameWithCurrentSelection()
    {
        if (carouselManager == null || carouselManager.ReadOnlySongList.Count == 0) return;

        SongData selectedSong = carouselManager.ReadOnlySongList[songSelect.currentIndex];
        if (selectedSong.difficulties == null || selectedSong.difficulties.Count == 0) return;

        // 유저가 고른 난이도를 곡 데이터에 저장
        var selectedPattern = selectedSong.difficulties[currentDiffListIndex];
        selectedSong.ChoiceSongDifficulty = selectedPattern.difficultyType;

        // 다음 씬까지 데이터를 들고 갈 DontDestroy 매니저에 확정 전달
        if (SongSelectDataManager.Instance != null)
        {
            SongSelectDataManager.Instance.SetSong(selectedSong);
        }
        else
        {
            Debug.LogWarning("[TrackInput] SongSelectDataManager가 씬에 없습니다!");
        }

        // 영상 유무 체크
        if (selectedSong.SongVideoClip == null)
        {
            Debug.LogWarning($"[TrackInput] '{selectedSong.title}'의 VideoClip이 비어있습니다.");
        }

        InSongLoad.Instance.StartSongLoading("GameTest", selectedSong.title, selectedSong.title_img, selectedSong.FmodEvent);

        Debug.Log($"[TrackInput] 선택된 곡: {selectedSong.title}, 난이도: {selectedSong.ChoiceSongDifficulty}, 파일 이름: {selectedSong.songFileName}, 영상 정보 : {selectedSong.SongVideoClip}");
    }

    public void OnNavigate(InputValue context)
    {
        Vector2 input = context.Get<Vector2>();
        if (input == Vector2.zero) return;

        int songCount = carouselManager.ReadOnlySongList.Count;
        if (songCount == 0) return;

        if (input.x != 0)
        {
            songSelect.MoveCarousel((int)input.x);

            ChangeSongUI(songSelect.currentIndex);
        }

        if (input.y != 0)
        {
            var currentSong = carouselManager.ReadOnlySongList[songSelect.currentIndex];
            int diffCount = currentSong.difficulties.Count;

            if (diffCount > 0)
            {
                currentDiffListIndex += (input.y > 0 ? -1 : 1);

                if (currentDiffListIndex < 0) currentDiffListIndex = diffCount - 1;
                else if (currentDiffListIndex >= diffCount) currentDiffListIndex = 0;

                // 하이라이트 위치 설정
                var selectedPattern = currentSong.difficulties[currentDiffListIndex];
                difficultSelect.difficult = (int)selectedPattern.difficultyType;

                SongDetailInfo.Instance.UpdateDifficulty(currentDiffListIndex);
            }
        }
    }
}