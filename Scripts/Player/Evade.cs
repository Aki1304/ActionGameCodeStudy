using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Evade : MonoBehaviour
{
    private Animator _animator;
    private Character _character;

    #region ������Ƽ
    public bool _evade { get; private set; }
    private PlayerManager PM { get { return PlayerManager.Instance; } }
    private bool UsingSkill { get { return _character.GetSkillUsing; } set { _character.GetSkillUsing = value; } }
    public KeySetting GetKeySet { get { return GameManager.Instance.KeySet; } }
    #endregion

    private void Start()
    {
        EvadeInit();
    }

    void EvadeInit()
    {
        _character = GetComponent<Character>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        EvadeInput();
    }


    void EvadeInput()
    {
        if (Input.GetKeyDown(GetKeySet._evadeKey))
        {
            if (!GetKeySet.GetMouseIsLock || _character._anyState) return;

            int indexNum = 1;       // �� �Է½� �ڷ�
            if (GetKeySet.EvadeMoveKeys.Any(Input.GetKey))
            {
                indexNum = 0;
            }

            if(_character.GetNormalAttackBool) { indexNum = 1; }

            EvadeUse(indexNum);
        }
    }

    bool CheckPossibleUse()
    {
        // ���¹̳��� ��뿩�� üũ
        return _character.ReturnCheckStamina(PM.GetSkillData[1].SkillStaminaUse);
    }

    void EvadeUse(int idx)
    {
        if (!CheckPossibleUse()) { Debug.Log("���¹̳� false"); return; }

        string evadeName = PM.GetSkillData[1].SkillName;
        string Under = "_";
        string Idle = "Idle";
        // Vector�� WAS �ƹ��ų� ������ �� ������ ���� �Ǽ� WAS�� ���� ���� W, �� �ܴ̿� D�� ó��
        string Vector = idx switch
        {
            0 => "W",
            _ => "S",
        };

        bool firstCheck = false;
        bool secondChbeck = false;


        if (!UsingSkill) SetEvade();

        void SetEvade()
        {
            UsingSkill = true;
            _evade = true;

            SetSkill();
            StartCoroutine(SkillAnim());
        }

        void SetSkill()
        {

            _character.SetStop();
            _animator.SetBool(evadeName, true);
            _animator.SetInteger("Evade_V", idx);
            _character.UseStamina((int)PM.GetSkillData[1].SkillStaminaUse);
        }

        IEnumerator SkillAnim()
        {
            yield return null;

            yield return new WaitUntil(() => GetAnimatorPlaying());

            _animator.SetBool(evadeName, false);
            UsingSkill = false;

            bool GetAnimatorPlaying()
            {
                if (_character.GetHit) return true;

                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(2);
                float animTime = stateInfo.normalizedTime;

                if (stateInfo.shortNameHash == Animator.StringToHash($"{evadeName}{Under}{Vector}"))
                {
                    if(animTime >= 0.5f ) _evade = false;
                    firstCheck = true;
                }
                if (stateInfo.shortNameHash == Animator.StringToHash($"{evadeName}{Under}{Idle}")) secondChbeck = true;
                if (animTime >= 1.0f && ReturnCheck()) return true; else return false;

            }

            bool ReturnCheck()
            {
                return firstCheck && secondChbeck;
            }
        }
    }

}
