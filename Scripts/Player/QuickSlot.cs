using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    #region ������ �迭
    [HideInInspector] public SkillData[] QuickSlotSkills = new SkillData[5];  // ���� ũ�� 5
    #endregion

    #region ������Ƽ
    private Player _player { get { return PlayerManager.Instance.GetPlayer; } }
    private QuickSlotUI _quikUI { get { return UIManager.Instance.GetGameUI.GetSkillTab.GetQuickUI; } }
    private WindowUI GetWindow { get { return UIManager.Instance.GetWindowUI; } }
    private KeySetting GetKeySet { get { return GameManager.Instance.KeySet; } }
    public bool _quickCheck { get; private set; }           // ������ ����
    #endregion

    void InitQuick()
    {
        _quickCheck = false;
    }
    void KeyInput()                             // key Input ������ ��� �뵵
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
        // ���࿡ �̹� �����Ѵٸ�
        for(int i = 0; i < QuickSlotSkills.Length; i++)
        {
            if (QuickSlotSkills[i] == data) // �̹� ������ ���� �������� �����ϸ�
            {
                QuickSlotSkills[i] = null;
                _quikUI.SpriteChange(i, false);
                break;
            }   
        }

        QuickSlotSkills[number] = data;                         // ���
        _quikUI.SpriteChange();                                 // �̹��� ���
    }
}
