using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPerkPassive", menuName = "Scriptable Object/Perks/AttackPerkPassive")]

public class AttackPerkPassive : PerkEffect
{

    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 3;                      // 패시브 상승  최대 레벨

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

        return $"현재 공격력의 {_curLevel * _passivePercent}% 만큼 증가한다.";
    }

    public override string StringToPickToolTip()
    {
        int perccent = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;

        return $"현재 공격력의 {perccent * _passivePercent}% 만큼 증가한다.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
