using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPerkPassive", menuName = "Scriptable Object/Perks/AttackPerkPassive")]

public class AttackPerkPassive : PerkEffect
{

    public int _curLevel = 0;                            // �нú� ��� ���� ����

    public const int _maxLevel = 3;                      // �нú� ���  �ִ� ����

    public int _passivePercent = 5;

    public string _skillEX;

    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;

        PlayerManager PM = PlayerManager.Instance;
        Player player = PM.GetPlayer;

        player.charStats._perkAtk = (_curLevel * _passivePercent) * 0.01f;
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {

        return $"���� ���ݷ��� {_curLevel * _passivePercent}% ��ŭ �����Ѵ�.";
    }

    public override string StringToPickToolTip()
    {
        int perccent = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;

        return $"���� ���ݷ��� {perccent * _passivePercent}% ��ŭ �����Ѵ�.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
