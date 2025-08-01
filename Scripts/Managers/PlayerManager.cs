using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class PlayerManager : SingleTon<PlayerManager>
{
    [Header("플레이어 스크립트")]
    [SerializeField, Tooltip("플레이어_건")] private Player _player;

    
    #region 프로퍼티
    public Player GetPlayer { get { return _player; } }
    public List<SkillData> GetSkillData { get { return _player.GetPlayerSkill.GetSkillDataList;  } }
    public int GetPlayerSkillPoint { get { return _player.GetPlayerSkill.ReturnSkillPoint(); } }
    #endregion

    #region 플레이언 스킬 퀵슬룻 배열 
    public void UpdateQuickSlot(SkillData data, int number)
    {
        // UI 매니저에게 플레이어 UI에도 전달이 되야한다.
        // 플레이어 퀵슬룻 배열도 업데이트가 되어야 한다.
        GetPlayer.GetQuickSlot.UpdateQuickSlot(data, number);
        Debug.Log(string.Format("단축키 등록 : {0} 퀵슬룻 넘버 {1}",data.InSkillName, number));

    }
    #endregion
}
