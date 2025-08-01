using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterHP
{
    #region ��ü ��������
    // CharacterStatus ��������
    private CharacterStatus _status;

    #endregion

    public void GetStatus(CharacterStatus stats) => _status = stats;        // �÷��̾� ��ü�� ü�� ���� ��������
    public int currentHP { get { return _status._curHP; } set { _status._curHP = value; } }
    public int maxHP { get { return _status._maxHP; }  }
    public float currentStamina { get { return _status._curStamina; } set { _status._curStamina = value; } }
    public int maxStamina { get { return _status._maxStamina; }  }



    public bool IsStaminaMoving { get; set; }

    public float lastUseTime { get; set; }                       // ���������� ���¹̳��� ���� �ð�
    public float recoveryDelay = 2f;                // �� �� �Ŀ� ȸ������

    public int SetPlayerHP     // �� ĳ������ ���� ü��
    {
        get { return this.currentHP; }
        set { currentHP = value > maxHP ? maxHP : value; }
    }

    public void CharHPInit(CharacterStatus stats)
    {
        _status = stats;

        _status._defaultHP = 100;
        _status._defaultStamina = 100;

        // ü�� ���׹̳� ����
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    //public void CharHPChange(int changeStats) => maxHP = maxHP + changeStats;

    //public void CharStaminaChange(int changeStats)
    //{
    //    // ���¹̳��� ���� �� �ٷ� Ǯ�� ä���ֱ�
    //    maxStamina = maxStamina + changeStats;
    //    currentStamina = (float)maxStamina;
    //}

    public void GetHeal(int healPercent)            
    {
        currentHP = (maxHP <= (healPercent + currentHP)) ? maxHP : currentHP + healPercent;
    }

    public void TakeDamage(int takeDmg)                     // ������ ������ ����� �ٲٱ�
    {
        // ���� ü�°� ���ų� ���� �������� ������.
        currentHP = (takeDmg >= currentHP) ? 0 : currentHP - takeDmg;
    }

    public void UseStamina(int stamina)
    {
        currentStamina = (stamina >= currentStamina) ? 0 : currentStamina - stamina;
        lastUseTime = Time.time;
    }

}
