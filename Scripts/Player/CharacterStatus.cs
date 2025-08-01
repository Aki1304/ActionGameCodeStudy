using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus
{
    public int _CodeForUnit { get; private set; }
    public string _charName { get; set; }
    public int _maxHP { get { return _defaultHP + passiveHP; } }
    public int _curHP { get; set; }
    public int _maxStamina { get { return _defaultStamina + passiveStamina; } }
    public float _curStamina { get; set; }
    public int _atkDmg { get; set; }
    public int _defDmg { get; set; }
    public float _criPercent { get; set; }
    public int _walkSpeed { get; set; }
    public int _runSpeed { get; set; }
    public int _defaultHP { get; set; }
    public int _defaultStamina { get; set; }

    // 버프 프로퍼티
    public float _buffAtk { get; set; }
    public float _buffCri { get; set; }


    // 퍽 전용 계산
    public float _perkAtk { get; set; }
    public float _perkCritical { get; set; }
    public int _perkNoraml {  get; set; }
    public int _perkHeal { get; set; }  
    public int _perkstamina { get; set; }


    // 버프 계산식 
    #region 버프 계산식
    public float Attack
    { 
        get
        {
            int baseatk = _atkDmg + Mathf.RoundToInt(_atkDmg * _buffAtk);
            int buffatk = Mathf.RoundToInt(baseatk * _buffAtk);
            return baseatk + buffatk;
        } 
    }
    public float Critical
    { 
        get { float critical = _criPercent + _perkCritical + _buffCri; return (critical >= 100) ? 100 : critical; }
    }
    public int passiveHP { get { return GetSkilldata[2].SkillPassivePercent * GetSkilldata[2].SkillCurLv; } }
    public int passiveStamina { get { return GetSkilldata[3].SkillPassivePercent * GetSkilldata[3].SkillCurLv; } }

    public List<SkillData> GetSkilldata { get { return PlayerManager.Instance.GetSkillData; } }
    #endregion


    /// <summary>
    /// 플레이어 0, 일반 몬스터 1, 보스 몬스터 2
    /// 각 스탯에 따라 들어가는 데미지 배율이 다르게 할 것,
    /// 아후 공격 공식 스크립트에서 이를 반영해서 적용
    /// </summary>
    /// <param name="code"></param>
    public void SetCharCode(int code) => this._CodeForUnit = code;

    public void SetCharStat(string name, int atkDmg, int defDmg,
        int curHp = 0,  float curStamina = 0, int criPercent = 0, int walkSpeed = 3, int runSpeed = 6)
    {
        this._charName = name;
        this._curHP = curHp;
        this._curStamina = curStamina;
        this._atkDmg = atkDmg;
        this._defDmg = defDmg;
        this._criPercent = criPercent;          // 일반 적인 크리가 아닌 추가공격 확률 느낌
        this._walkSpeed = walkSpeed;
        this._runSpeed = runSpeed;
    }

}


