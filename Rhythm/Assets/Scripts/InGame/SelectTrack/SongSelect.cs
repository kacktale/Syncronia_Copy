using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    public RectTransform[] panels = new RectTransform[5];
    private SongSelectBtn[] btnScripts = new SongSelectBtn[5];

    public Vector2[] fixedPositions = {
        new Vector2(-1200, 155), new Vector2(-600, 155), new Vector2(0, 155), new Vector2(600, 155), new Vector2(1200, 155)
    };
    public float[] fixedScales = { 0.8f, 0.9f, 1.1f, 0.9f, 0.8f }; // 가운데만 1.1배

    private int[] panelTargetIndices = { 0, 1, 2, 3, 4 };
    public int currentIndex = 0;
    public SongCarouselManager carouselManager;

    private void Awake()
    {
        for (int i = 0; i < 5; i++) btnScripts[i] = panels[i].GetComponent<SongSelectBtn>();
    }

    public void InitCarousel(int startIndex)
    {
        currentIndex = startIndex;
        RefreshAllPanelData();
        for (int i = 0; i < 5; i++)
        {
            panels[i].anchoredPosition = fixedPositions[panelTargetIndices[i]];
            panels[i].localScale = Vector3.one * fixedScales[panelTargetIndices[i]];
        }
    }

    public void MoveCarousel(int direction)
    {
        int total = carouselManager.ReadOnlySongList.Count;
        if (total == 0) return;

        // 곡 인덱스 이동
        currentIndex = (currentIndex + direction + total) % total;

        for (int i = 0; i < 5; i++)
        {
            // 목표 자리 이동
            panelTargetIndices[i] = (panelTargetIndices[i] - direction + 5) % 5;

            // 끝에서 반대편으로 순간이동한 패널에게 새 데이터 입히기
            if (direction == 1 && panelTargetIndices[i] == 4)
                UpdatePanelData(i, currentIndex + 2);
            else if (direction == -1 && panelTargetIndices[i] == 0)
                UpdatePanelData(i, currentIndex - 2);
        }
    }

    private void RefreshAllPanelData()
    {
        for (int i = 0; i < 5; i++)
        {
            int offset = panelTargetIndices[i] - 2;
            UpdatePanelData(i, currentIndex + offset);
        }
    }

    private void UpdatePanelData(int panelIndex, int targetSongIndex)
    {
        int total = carouselManager.ReadOnlySongList.Count;
        if (total == 0) return;
        int safeIndex = (targetSongIndex % total + total) % total;
        btnScripts[panelIndex].Setup(carouselManager.ReadOnlySongList[safeIndex]);
    }

    public SongSelectBtn GetCenterButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (panelTargetIndices[i] == 2) return btnScripts[i];
        }
        return null;
    }

    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            int target = panelTargetIndices[i];
            panels[i].anchoredPosition = Vector2.Lerp(panels[i].anchoredPosition, fixedPositions[target], Time.deltaTime * 15f);
            panels[i].localScale = Vector3.Lerp(panels[i].localScale, Vector3.one * fixedScales[target], Time.deltaTime * 15f);
        }
    }

    //Jira Test용 주석 ++
}
