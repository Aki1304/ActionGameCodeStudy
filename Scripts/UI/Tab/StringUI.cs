using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointUseUI { None, Possible, MaxLev, Impossible }
public class StringUI
{
    #region ������Ƽ
    private PlayerManager _pm { get { return PlayerManager.Instance; } }
    #endregion

    public string ReturnKor(SkillClass textClass)
    {
        return textClass switch
        {
            SkillClass.AttackSkill => "����",
            SkillClass.Buff => "����",
            SkillClass.Active => "��Ƽ��",
            SkillClass.Passive => "�нú�",
            _ => "X"
        };
    }

    public string ReturnWindowString(PointUseUI stat, SkillData data)
    {
        return stat switch
        {
            PointUseUI.Impossible => "��ų����Ʈ�� �����ϴ�.",
            PointUseUI.Possible => string.Format("[{0}]\n\n" + "{1}  =>  {2}  ��ų����Ʈ�� ��� �Ǿ����ϴ�.",
            data.InSkillName, _pm.GetPlayerSkillPoint, _pm.GetPlayerSkillPoint - 1),
            PointUseUI.MaxLev => "�̹� ��ų�� ������ �߽��ϴ�.",
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
            false => "��ų�� ��� �� �� �����ϴ�.",
            true => "����Ű ��ȣ�� �����ּ���.\n [1]  [2]  [Q]  [E]  [R]"
        };
    }
}
