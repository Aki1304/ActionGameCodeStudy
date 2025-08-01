using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPointPerk", menuName = "Scriptable Object/Perks/SkillPointPerk")]

public class SkillPointPerk : PerkEffect
{

    public int _curLevel = 0;                            // �нú� ��� ���� ����

    public const int _maxLevel = 25;                      // �нú� ���  �ִ� ����

    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;

        for (int i = 0; i < 2; i++) PlayerManager.Instance.GetPlayer.GetPlayerSkill.UpSkillPoint();
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"���� ��ų ����Ʈ�� {_curLevel * 2} ��ŭ ȹ���߽��ϴ�.";
    }

    public override string StringToPickToolTip()
    {
        return $"��ų ����Ʈ�� 2�� ȹ���մϴ�.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
