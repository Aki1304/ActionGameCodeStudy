using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player _player;
    private Animator _animator;

    private float timeDelayCheck;

    public bool GetDelayNormalAttack { get; private set; }
    public bool GetRunWalkCheck { get; private set; }
    public GameManager GM { get { return GameManager.Instance; } }

    private void Start()
    {
        InitClass();
    }

    public  void Update()
    {
        SetAnyState();
        SetWalkRunAnim();
        SetNormalActiveBool();
        SetSkillAnimBool();

        //CharNormalAttackCombo();
    }


    #region 애니메이터 사용 변수
    private float _normalAttackSpeed = 1.15f; // 기본속도 배수 값   

    private float GetNormalAttackSpeed
    {
        get
        {
            float percent = _normalAttackSpeed * GM.GetPM.GetNormalAttackPerk;
            return _normalAttackSpeed + percent;
        }
    }
    #endregion


    private void SetWalkRunAnim() => _animator.SetFloat("Speed", _player._isSpeed);
    private void SetAnyState() => _animator.SetBool("AnyState", _player._anyState);
    protected void SetBackIdle() { _player.SetStop(); _animator.SetTrigger("BackIdle"); }
    protected void SetNormalActiveBool() => _animator.SetBool("BNormalAttack", _player._normalAttackBool);
    protected void SetNormalAttackAnim() => _animator.SetInteger("INormalAttack", GetAnimState());


    private void SetSkillAnimBool()
    {
        if (ReturnCheck()) _animator.SetBool("Skill", _player.GetSkillUsing);

        bool ReturnCheck()
        {
            return !_player._anyState || !_player._normalAttackBool;
        }
    }

    private int GetAnimState()
    {
        switch (_player.GetAnimState.GetNormalAttackState)
        {
            case PlayerAnimState.NormalAttackAnimState.Attack1:
                return 0;
            case PlayerAnimState.NormalAttackAnimState.Attack2:
                return 1;
            case PlayerAnimState.NormalAttackAnimState.Attack3:
                return 2;
            default: return 0;
        }
    }


    private void InitClass()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();

        GetDelayNormalAttack = false;
    }

    //public void CharNormalAttackAnim()
    //{
    //    if(!_player._normalAttackBool)
    //    {
    //        _player._normalAttackBool = true;
    //        GetDelayNormalAttack = true;

    //        SetNormalAttackAnim();
    //    }

    //}

    //public void CharNormalAttackCombo()
    //{
    //    if(GetDelayNormalAttack)
    //    {
    //        if (_player.GetHit)
    //        {
    //            _player._normalAttackReClick = false;
    //            GetDelayNormalAttack = false;
    //            CheckReClick();
    //        }

    //        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    //        float time = stateInfo.normalizedTime;

    //        if (time >= 1.0f)
    //        { GetDelayNormalAttack = false; CheckReClick(); }
    //        else
    //        {
    //            if(time >= 0.25f && time <= 0.95f)                  // 이 조건 안에 다시 누른다면
    //            {
    //                if (Input.GetKeyDown(KeyCode.Mouse0))
    //                {
    //                    _player._normalAttackReClick = true;
    //                }
    //            }
    //        }
    //    }
    //}

    //public void CheckReClick()
    //{
    //    NextCombo();
    //    if (_player._normalAttackReClick)
    //    {
    //        // 성공
    //        _player._normalAttackReClick = false;
    //        _player._normalAttackBool = true;
    //        GetDelayNormalAttack = true;

    //        SetNormalAttackAnim();
    //    }
    //    else
    //    {
    //        // 실패
    //        SetBackIdle();
    //        Invoke("InvokeDelay",0.15f);

    //    }
    //}

    //private void NextCombo()
    //{
    //    int nextNum = (int)_player.GetAnimState.GetNormalAttackState + 1;
    //    if (nextNum > 2) nextNum = 0;

    //    _player.GetAnimState.SetNAState((PlayerAnimState.NormalAttackAnimState)nextNum);

    //    if(!_player._normalAttackReClick) _player.GetAnimState.SetNAState((PlayerAnimState.NormalAttackAnimState)0);
    //}

    //private void InvokeDelay()
    //{
    //    _player._normalAttackBool = false;
    //    _player._normalAttackReClick = false;
    //}

    public void CharNormalAttackAnim()
    {
        if (GetDelayNormalAttack) return;

        if (!_player._normalAttackBool)
        {
            _player._normalAttackBool = true;
            SetIntAnim();
        }

        void SetIntAnim()
        {
            if (!_player._normalAttackBool) return;

            //Debug.Log("더블클릭 검사");

            _player._normalAttackReClick = false;
            GetDelayNormalAttack = true;
            _animator.SetFloat("AttackSpeed", GetNormalAttackSpeed);
            StartCoroutine(DelayAttack());
        }

        IEnumerator DelayAttack()
        {
            yield return null;

            SetNormalAttackAnim();
            _player.GetRotateEnemy();

            yield return new WaitUntil(() => ReturnCheckAnim());
            yield return new WaitUntil(() => GetAnimatorPlaying());

            NextCombo();

            yield return null;
        }

        int ReturnNormalAttackState(PlayerAnimState.NormalAttackAnimState state)
        {
            return (int)_player.GetAnimState.GetNormalAttackState;
        }

        bool ReturnCheckAnim()
        {
            // 구르기나 스킬 사용 중이라면
            if (_player.GetSkillUsing || _player._anyState)
            {
                _player._normalAttackReClick = false;
                return true;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            int stateHash = stateInfo.shortNameHash;

            int number = ReturnNormalAttackState((_player.GetAnimState.GetNormalAttackState));
            string animName = string.Format("Normal_Attack_0{0}", number + 1);

            if (stateHash == Animator.StringToHash(animName)) return true;
            else return false;
        }

        bool GetAnimatorPlaying()                           // ReClick 및 다른 상태 감지
        {
            // 구르기나 스킬 사용 중이라면
            if (_player.GetSkillUsing || _player._anyState)
            {
                _player._normalAttackReClick = false;
                return true;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animTime = stateInfo.normalizedTime;

            if (animTime >= 1.0f) { GetDelayNormalAttack = true; return true; }
            else
            {
                if (animTime >= 0.25f && animTime <= 0.92f)                 // 이 간격 안에 마우스 좌클릭을 눌렀다면 
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        //Debug.Log("입력감지");
                        _player._normalAttackReClick = true;
                    }
                }

                return false;
            }
        }

        void NextCombo()
        {
            int nextNum = (int)_player.GetAnimState.GetNormalAttackState + 1;
            if (nextNum > 2) nextNum = 0;
            bool isLast = (nextNum == 0);

            _player.GetAnimState.SetNAState((PlayerAnimState.NormalAttackAnimState)nextNum);

            if (!_player._normalAttackReClick) _player.GetAnimState.SetNAState((PlayerAnimState.NormalAttackAnimState)0);

            if(!_player._normalAttackReClick || isLast || _player.GetSkillUsing || _player._anyState)
            {
                // 실패   
                if(!_player._anyState) SetBackIdle();                                          // backIdle이 필요한 애들임
                Invoke("InvokeDelay", 0.15f);
                return;
            }

            if (_player._normalAttackReClick)
            {
                // 성공
                SetIntAnim();
            }
        }

    }

    private void InvokeDelay()
    {
        _player._normalAttackBool = false;
        GetDelayNormalAttack = false;

    }


    public void AnyStateCheckAnim(PlayerAnimState.AnyState state)
    {
        if (state == PlayerAnimState.AnyState.Hit) { _player.GetHit = true; _animator.SetTrigger("Hit"); }

        _player.SetInputAnyState(true);

        StartCoroutine(AnimatorCheck());

        IEnumerator AnimatorCheck()
        {
            yield return null;

            yield return new WaitUntil(() => ReturnAnimTime());

            _player.SetInputAnyState(false);
            _player.GetHit = false;

            bool ReturnAnimTime()
            {
                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                int hash = stateInfo.shortNameHash;

                if(PlayerAnimState.AnyState.Hit == state)
                {
                    if (hash == Animator.StringToHash("Hit"))
                    {
                        if (stateInfo.normalizedTime >= 0.7f) return true;
                    }
                }

                return false;
            }

        }
    }
}
