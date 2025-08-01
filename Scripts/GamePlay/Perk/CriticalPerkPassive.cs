using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CriticalPerkPassive", menuName = "Scriptable Object/Perks/CriticalPerkPassive")]

public class CriticalPerkPassive : PerkEffect
{

    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 5;                      // 패시브 상승  최대 레벨

    public float _passivePercent = 5.5f;

    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;

        PlayerManager PM = PlayerManager.Instance;
        Player player = PM.GetPlayer;

        player.charStats._perkCritical = _passivePercent * _curLevel;
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"크리티컬이 {_curLevel * _passivePercent}% 만큼 증가합니다.";
    }

    public override string StringToPickToolTip()
    {
        int percent = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"크리티컬이 {percent * _passivePercent}% 만큼 증가합니다.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
