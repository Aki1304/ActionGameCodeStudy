using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttackPassive", menuName = "Scriptable Object/Perks/NormalAttackPassive")]

public class NormalAttackPassive : PerkEffect
{
    public int _curLevel = 0;                            // �нú� ��� ���� ����

    public const int _maxLevel = 3;                      // �нú� ���  �ִ� ����

    public float _passivePercent = 0.05f;

    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;

        PerkManager PM = GameManager.Instance.GetPM;
        PM.GetNormalAttackPerk = _curLevel * 0.05f;
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"���� �⺻ ���ݼӵ��� {_curLevel * 5}%�� ���� ��ŵ�ϴ�.";
    }

    public override string StringToPickToolTip()
    {
        int percent = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"���� �⺻ ���ݼӵ��� {percent * 5}%�� ���� ��ŵ�ϴ�.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
