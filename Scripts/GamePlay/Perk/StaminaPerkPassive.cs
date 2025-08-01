using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaPerkPassive", menuName = "Scriptable Object/Perks/StaminaPerkPassive")]

public class StaminaPerkPassive : PerkEffect
{

    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 5;                      // 패시브 상승  최대 레벨

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
        return $"현재 스태미나 회복량을 {_curLevel * 3}% 만큼씩 상승시킵니다.";
    }

    public override string StringToPickToolTip()
    {
        int level = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"현재 스태미나 회복량을 {level * 3}% 만큼씩 상승시킵니다.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
