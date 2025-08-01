using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaPerkPassive", menuName = "Scriptable Object/Perks/StaminaPerkPassive")]

public class StaminaPerkPassive : PerkEffect
{

    public int _curLevel = 0;                            // �нú� ��� ���� ����

    public const int _maxLevel = 5;                      // �нú� ���  �ִ� ����

    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;

        UIManager.Instance.GetPlayerUI.GetStaminaPuckLevel = _curLevel;
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"���� ���¹̳� ȸ������ {_curLevel * 3}% ��ŭ�� ��½�ŵ�ϴ�.";
    }

    public override string StringToPickToolTip()
    {
        int level = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"���� ���¹̳� ȸ������ {level * 3}% ��ŭ�� ��½�ŵ�ϴ�.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
