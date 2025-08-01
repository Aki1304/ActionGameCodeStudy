using JetBrains.Annotations;
using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class PlayerSkill : MonoBehaviour
{
    #region 객체 생성
    private Character _character;
    private QuickSlot _quickSlot;
    private PlayerBuffBuffer _buffBuffer;

    [Header("스킬 포인트")]
    [SerializeField] private int _skillPoint;
    #endregion

    #region 프로퍼티
    public PlayerManager _pm { get { return PlayerManager.Instance; } }
    private bool UsingSkill { get { return _character.GetSkillUsing; } set { _character.GetSkillUsing = value; } }
    #endregion

    #region 스킬 리스트
    [Header("스킬 리스트")]
    [SerializeField, Tooltip("스킬 리스트 모음")] private List<SkillData> _listSkillData;
    public List<SkillData> GetSkillDataList { get { return _listSkillData; } }
    #endregion

    #region 초기화 및 애니 업데이트
    protected void Awake()
    {
        InitPlayerSkill();
    }
    private void InitPlayerSkill()
    {
        _character = GetComponent<Character>();
        _quickSlot = GetComponent<QuickSlot>();
        _buffBuffer = GetComponent<PlayerBuffBuffer>();
    }
    #endregion

    #region 스킬 포인트
    public bool ReturnPossibleSkillRankUP() => (_skillPoint > 0) ? true : false;
    public int ReturnSkillPoint() => _skillPoint;
    public void DownSkillPoint() => _skillPoint--;
    public void UpSkillPoint() => _skillPoint++;
    #endregion

    public void GetUseSkill(Animator animator, int number)
    {
        SkillData selectSkillData = _quickSlot.QuickSlotSkills[number];
        SkillClass classCheck = selectSkillData.SkillClass;
        string isClass = ClassName(selectSkillData.SkillClass);

        int activeSkillNum = selectSkillData.AttackSkillNum - 1;            // 액티브 스킬 숫자..
        float stamina = selectSkillData.SkillStaminaUse;

        // 현재 스태미나 체크 후 없으면 못하게
        if (_character.charStats._curStamina < stamina) return;

        if (UsingSkill) return;

        UsingSkill = true;
        _character.SetStop();

        SetTriggerSkill();
        StartCoroutine(SkillAnim());

        void SetTriggerSkill()
        {
            if (classCheck == SkillClass.Buff) _buffBuffer.BuffEffect(number);
            if (classCheck == SkillClass.AttackSkill) animator.SetInteger("SkillNumber", activeSkillNum);

            _character.UseStamina(selectSkillData.SkillStaminaUse);

            animator.SetTrigger(isClass); 
            Debug.Log(isClass);
        }

        IEnumerator SkillAnim()
        {
            yield return null;

            yield return new WaitUntil(() => GetAnimatorPlaying());

            Debug.Log("스킬 끝");

            UsingSkill = false;

            bool GetAnimatorPlaying()
            {
                if (_character.GetHit) return true;

                string under = "_";
                string BackIdle = "Back_Idle";


                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
                float animTime = stateInfo.normalizedTime;


                if (classCheck == SkillClass.Buff)          // 버프 인 경우는 모션 때문에 두 번 
                {
                    if (stateInfo.shortNameHash == Animator.StringToHash($"{isClass}{under}{1}"))
                    {
                        if (animTime >= 1.0f) return true;
                    }
                }

                if (classCheck == SkillClass.AttackSkill)
                {
                    if (stateInfo.shortNameHash == Animator.StringToHash(BackIdle))
                    {
                        if (animTime >= 1.0f) return true;
                    }
                }

                return false;
            }
        }
    }


    private string ClassName(SkillClass isClass)
    {
        return isClass switch
        {
            SkillClass.Passive => "Passive",
            SkillClass.Buff => "Buff",
            SkillClass.AttackSkill => "Attack Skill",
            _ => "None"
        };
    }
}
