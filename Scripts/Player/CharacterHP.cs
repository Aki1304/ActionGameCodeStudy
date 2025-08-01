using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterHP
{
    #region 객체 가져오기
    // CharacterStatus 가져오기
    private CharacterStatus _status;

    #endregion

    public void GetStatus(CharacterStatus stats) => _status = stats;        // 플레이어 객체의 체력 스탯 가져오기
    public int currentHP { get { return _status._curHP; } set { _status._curHP = value; } }
    public int maxHP { get { return _status._maxHP; }  }
    public float currentStamina { get { return _status._curStamina; } set { _status._curStamina = value; } }
    public int maxStamina { get { return _status._maxStamina; }  }



    public bool IsStaminaMoving { get; set; }

    public float lastUseTime { get; set; }                       // 마지막으로 스태미나가 사용된 시간
    public float recoveryDelay = 2f;                // 몇 초 후에 회복할지

    public int SetPlayerHP     // 각 캐릭터의 현재 체력
    {
        get { return this.currentHP; }
        set { currentHP = value > maxHP ? maxHP : value; }
    }

    public void CharHPInit(CharacterStatus stats)
    {
        _status = stats;

        _status._defaultHP = 100;
        _status._defaultStamina = 100;

        // 체력 스테미나 적용
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    //public void CharHPChange(int changeStats) => maxHP = maxHP + changeStats;

    //public void CharStaminaChange(int changeStats)
    //{
    //    // 스태미나는 변경 시 바로 풀로 채워주기
    //    maxStamina = maxStamina + changeStats;
    //    currentStamina = (float)maxStamina;
    //}

    public void GetHeal(int healPercent)            
    {
        currentHP = (maxHP <= (healPercent + currentHP)) ? maxHP : currentHP + healPercent;
    }

    public void TakeDamage(int takeDmg)                     // 데미지 공식이 생기면 바꾸기
    {
        // 현재 체력과 같거나 높은 데미지를 입으면.
        currentHP = (takeDmg >= currentHP) ? 0 : currentHP - takeDmg;
    }

    public void UseStamina(int stamina)
    {
        currentStamina = (stamina >= currentStamina) ? 0 : currentStamina - stamina;
        lastUseTime = Time.time;
    }

}
