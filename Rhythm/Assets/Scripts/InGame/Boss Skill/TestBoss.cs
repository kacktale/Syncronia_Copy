using UnityEngine;

public class TestBoss : BossSkill
{
    public override void BossSkillStart()
    {
        Debug.Log("보스 스킬 호출됨");
    }
}
