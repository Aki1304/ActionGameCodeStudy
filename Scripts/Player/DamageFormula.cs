using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageFormula
{
    public PlayerManager PM { get { return PlayerManager.Instance; } }

    public float _playerDamage { get { return PM.GetPlayer.charStats.Attack; } }
    public float _playerCritical { get { return PM.GetPlayer.charStats.Critical; } }



    public int EnemyTakeDamage(float EnemyDef, bool HitCritical = false)
    {
        float defencePercent = EnemyDef / (100f + EnemyDef);
        float damage = (HitCritical) ? _playerDamage * 1.5f : _playerDamage;

        //Debug.Log($"�� ����� {defencePercent}");

        float lastDamage;

        if (defencePercent >= 0.75f)
        {
            // 75%�� ������� �Ѵ´ٸ� �ּ� ������ ����
            lastDamage = damage * (1 - 0.75f);
        }
        else
        {
            // �ƴ� ����..
            lastDamage = damage * (1 - defencePercent);
        }


        return Mathf.RoundToInt(lastDamage);
    }
    

    public bool CheckCritical()
    {
        float random = UnityEngine.Random.Range(0, 100f);

        if (_playerCritical >= random)
        {
            return true;
        }
        else return false;
    }
}
