using UnityEngine;
using UnityEngine.EventSystems;

public class BossUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, HideInInspector] private BossSkillType bossSkillType;
    private bool isSetBossSkillType = false;

    [SerializeField, HideInInspector] private BossSkillSettingPanel bossPenel;

    /// <summary>
    /// 보스 타입 설정
    /// </summary>
    /// <param name="bossSkillType">설정할 보스 타입</param>
    /// <param name="bossSkillSettingPanel">보스 선택시 꺼질 패널</param>
    public void SetBossSkillType(BossSkillType bossSkillType, BossSkillSettingPanel bossSkillSettingPanel)
    {
        if(isSetBossSkillType) return;

        this.bossSkillType = bossSkillType;
        isSetBossSkillType = true;
        this.bossPenel = bossSkillSettingPanel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EditorSave.instance.data.bossType = bossSkillType;
        Debug.Log($"보스 스킬 타입이 {bossSkillType}로 설정되었습니다.");
        // bossPenel.gameObject.SetActive(false); // 보스 패널을 강제로 닫으면 다시 안열리는 문제가 있음
    }
}
