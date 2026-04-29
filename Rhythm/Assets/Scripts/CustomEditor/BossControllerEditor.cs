using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossSkillSettingPanel))]
public class BossControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BossSkillSettingPanel bossSkillSettingPanel = (BossSkillSettingPanel)target;

        if (GUILayout.Button("Add Boss Skill"))
        {
            int bossCount = bossSkillSettingPanel.bossCount;
            bossCount++;

            if (!Enum.IsDefined(typeof(BossSkillType), bossCount)) // 보스 타입이 추가되었는지 아닌지 확인
            {
                Debug.LogWarning("아직 Enum BossSkillType에 추가된 보스 스킬이 없습니다. Enum에 보스 스킬을 추가해주세요.");
                return;
            }

            bossSkillSettingPanel.bossCount = bossCount;

            // 아이콘 위치와 간격, 프리팹, 컨텐츠 크기 등을 가져옴
            int IconPosX = bossSkillSettingPanel.IconPosX;
            int IconPosY = bossSkillSettingPanel.IconPosY;
            int IconDistanceX = BossSkillSettingPanel.IconDistanceX;
            int IconDistanceY = BossSkillSettingPanel.IconDistanceY;
            GameObject bossPrefab = bossSkillSettingPanel.bossPrefab;
            List<GameObject> bossPrefabs = bossSkillSettingPanel.bossPrefabs;
            RectTransform contentRect = bossSkillSettingPanel.bossSkillContent.GetComponent<RectTransform>();

            if (bossPrefab == null)
            {
                Debug.LogError("Boss Prefab is not assigned.");
                return;
            }

            Undo.RecordObject(bossSkillSettingPanel, "Add Boss Skill");
            Undo.RecordObject(contentRect, "Resize Content");

            if (bossPrefabs.Count % 8 == 0 && bossPrefabs.Count > 0) // 아이콘 8개마다 컨텐츠의 크기를 증가시킴(페이지를 넘길 수 있도록)
            {
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x + 1000, contentRect.sizeDelta.y);
            }
            if (bossPrefabs.Count % 4 == 0) // 아이콘 4개마다 다음 줄로 이동
            {
                IconPosX = 100 + (int)(contentRect.sizeDelta.x - 1000);
                IconPosY = IconPosY <= -250 ? -50 : IconPosY + IconDistanceY;
            }
            else // 일반적인 아이콘 간격
            {
                IconPosX = IconPosX + IconDistanceX + (int)bossPrefab.transform.position.x;
            }

            // 프리팹 생성 및 위치 설정
            GameObject instanceObject = (GameObject)PrefabUtility.InstantiatePrefab(bossPrefab, bossSkillSettingPanel.bossSkillContent.transform);
            instanceObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(IconPosX, IconPosY);

            // 보스 스킬 타입 설정
            instanceObject.GetComponent<BossUI>().SetBossSkillType((BossSkillType)bossCount, bossSkillSettingPanel);
            bossSkillSettingPanel.bossPrefabs.Add(instanceObject);

            bossSkillSettingPanel.IconPosX = IconPosX;
            bossSkillSettingPanel.IconPosY = IconPosY;

            // Undo 지원 (중요!)
            Undo.RegisterCreatedObjectUndo(instanceObject, "Create Prefab");

            // 선택 상태로 만들기
            Selection.activeGameObject = instanceObject;

            EditorUtility.SetDirty(bossSkillSettingPanel);
        }
    }
}
