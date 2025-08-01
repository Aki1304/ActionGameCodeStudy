using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttackPassive", menuName = "Scriptable Object/Perks/NormalAttackPassive")]

public class NormalAttackPassive : PerkEffect
{
    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 3;                      // 패시브 상승  최대 레벨

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
        return $"현재 기본 공격속도를 {_curLevel * 5}%씩 증가 시킵니다.";
    }

    public override string StringToPickToolTip()
    {
        int percent = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"현재 기본 공격속도를 {percent * 5}%씩 증가 시킵니다.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
