using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BossSkillType
{
    None = 0, TestBoss = 1,
}
public class BossSkillSettingPanel : PanelBase
{
    public override PanelType PanelType => PanelType.Editor;
    [Header("보스 스킬 셋팅값")]
    public int IconPosX = 0;
    public int IconPosY = -250;
    public const int IconDistanceX = 200;
    public const int IconDistanceY = -200;
    public int bossCount = 0;

    public GameObject bossSkillContent;
    public GameObject bossPrefab;
    public List<GameObject> bossPrefabs;

    [Header("페이지 이동 버튼")]
    [SerializeField] private Button pageRightButton;
    [SerializeField] private Button pageLeftButton;

    // TODO : 보스의 이미지와 script를 가져와서 보스의 서브 페이지와 연동시킬 수 있는 기능을 제작
    [Header("보스 스킬 셋팅값")]
    public string bossDiscription = null;

    protected override void Awake()
    {
        base.Awake();
        bossPrefabs = new List<GameObject>();

        pageRightButton.onClick.AddListener(() => PageRigthMovement());
        pageLeftButton.onClick.AddListener(() => PageLeftMovement());
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnShow()
    {
        
    }

    public void PageRigthMovement()
    {
        Debug.Log("Page Right");
        RectTransform contentRt = bossSkillContent.GetComponent<RectTransform>();
        if(contentRt.sizeDelta.x - 1000 >= contentRt.anchoredPosition.x)
        {
            contentRt.anchoredPosition = Vector2.zero;
        }
        else
        {
            contentRt.anchoredPosition -= new Vector2(1000, 0);
        }
    }

    public void PageLeftMovement()
    {
        Debug.Log("Page Left");
        RectTransform contentRt = bossSkillContent.GetComponent<RectTransform>();
        if (contentRt.anchoredPosition.x >= 0)
        {
            contentRt.anchoredPosition = new Vector2(contentRt.sizeDelta.x - 1000, 0);
        }
        else
        {
            contentRt.anchoredPosition += new Vector2(1000, 0);
        }
    }
}
