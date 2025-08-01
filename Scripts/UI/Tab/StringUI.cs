using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointUseUI { None, Possible, MaxLev, Impossible }
public class StringUI
{
    #region 프로퍼티
    private PlayerManager _pm { get { return PlayerManager.Instance; } }
    #endregion

    public string ReturnKor(SkillClass textClass)
    {
        return textClass switch
        {
            SkillClass.AttackSkill => "어택",
            SkillClass.Buff => "버프",
            SkillClass.Active => "액티브",
            SkillClass.Passive => "패시브",
            _ => "X"
        };
    }

    public string ReturnWindowString(PointUseUI stat, SkillData data)
    {
        return stat switch
        {
            PointUseUI.Impossible => "스킬포인트가 없습니다.",
            PointUseUI.Possible => string.Format("[{0}]\n\n" + "{1}  =>  {2}  스킬포인트가 사용 되었습니다.",
            data.InSkillName, _pm.GetPlayerSkillPoint, _pm.GetPlayerSkillPoint - 1),
            PointUseUI.MaxLev => "이미 스킬을 마스터 했습니다.",
            _ => "Error"
        };
    }

    public string ReturnStringEx(SkillData data)
    {
        return data.SkillClass switch
        {
            SkillClass.AttackSkill => string.Format(data.SkillEx,data.SkillAttackDamage, 10),
            SkillClass.Buff or SkillClass.Passive => string.Format(data.SkillEx, data.PercentFormula()),
            SkillClass.Active => data.SkillEx,
            _ => "X"
        };

    }

    public string ReturnQuickSlotString(bool active)
    {
        return active switch
        {
            false => "스킬을 등록 할 수 없습니다.",
            true => "단축키 번호를 눌러주세요.\n [1]  [2]  [Q]  [E]  [R]"
        };
    }
}
