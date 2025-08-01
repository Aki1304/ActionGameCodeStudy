using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    #region 퀵슬룻 배열
    [HideInInspector] public SkillData[] QuickSlotSkills = new SkillData[5];  // 고정 크기 5
    #endregion

    #region 프로퍼티
    private Player _player { get { return PlayerManager.Instance.GetPlayer; } }
    private QuickSlotUI _quikUI { get { return UIManager.Instance.GetGameUI.GetSkillTab.GetQuickUI; } }
    private WindowUI GetWindow { get { return UIManager.Instance.GetWindowUI; } }
    private KeySetting GetKeySet { get { return GameManager.Instance.KeySet; } }
    public bool _quickCheck { get; private set; }           // 퀵슬룻 전용
    #endregion

    void InitQuick()
    {
        _quickCheck = false;
    }
    void KeyInput()                             // key Input 퀵슬룻 사용 용도
    {
        if (!_quickCheck)
        {
            if (GetKeySet.QuickKeys.Any(Input.GetKeyDown))
            {
                int index = Array.FindIndex(GetKeySet.QuickKeys, Input.GetKeyDown);
                if (QuickSlotSkills[index] == null) return;
                _player.UseSkill(index);
            }
        }
    }

    private void Awake()
    {
        InitQuick();
    }

    private void Update()
    {
        KeyInput();
    }

    public void UpdateQuickSlot(SkillData data,int number)
    {
        // 만약에 이미 존재한다면
        for(int i = 0; i < QuickSlotSkills.Length; i++)
        {
            if (QuickSlotSkills[i] == data) // 이미 퀵슬룻에 넣을 퀵슬룻이 존재하면
            {
                QuickSlotSkills[i] = null;
                _quikUI.SpriteChange(i, false);
                break;
            }   
        }

        QuickSlotSkills[number] = data;                         // 등록
        _quikUI.SpriteChange();                                 // 이미지 등록
    }
}
