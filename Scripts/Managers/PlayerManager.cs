using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class PlayerManager : SingleTon<PlayerManager>
{
    [Header("�÷��̾� ��ũ��Ʈ")]
    [SerializeField, Tooltip("�÷��̾�_��")] private Player _player;

    
    #region ������Ƽ
    public Player GetPlayer { get { return _player; } }
    public List<SkillData> GetSkillData { get { return _player.GetPlayerSkill.GetSkillDataList;  } }
    public int GetPlayerSkillPoint { get { return _player.GetPlayerSkill.ReturnSkillPoint(); } }
    #endregion

    #region �÷��̾� ��ų ������ �迭 
    public void UpdateQuickSlot(SkillData data, int number)
    {
        // UI �Ŵ������� �÷��̾� UI���� ������ �Ǿ��Ѵ�.
        // �÷��̾� ������ �迭�� ������Ʈ�� �Ǿ�� �Ѵ�.
        GetPlayer.GetQuickSlot.UpdateQuickSlot(data, number);
        Debug.Log(string.Format("����Ű ��� : {0} ������ �ѹ� {1}",data.InSkillName, number));

    }
    #endregion
}
