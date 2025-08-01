using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPointPerk", menuName = "Scriptable Object/Perks/SkillPointPerk")]

public class SkillPointPerk : PerkEffect
{

    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 25;                      // 패시브 상승  최대 레벨

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
        return $"현재 스킬 포인트를 {_curLevel * 2} 만큼 획득했습니다.";
    }

    public override string StringToPickToolTip()
    {
        return $"스킬 포인트를 2개 획득합니다.";
    }

    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
