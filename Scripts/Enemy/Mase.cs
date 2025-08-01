using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Mase : Enemy
{
    public string TestText;

    public override void Update()
    {
        base.Update();

        EnemyStateMachine();
    }

    public override void EnemyInit()
    {
        base.EnemyInit();

        _enemyStats = new EnemyStats(TestText, 500, 20, 38, 1f, 3.5f);

        // 속도 값 설정해주기
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.speed = _enemyStats.moveSpeed;

        // 초기 설정값 잡아주기
        _enemyState = EnemyState.Patrol;
    }

    public override void EnemyStateMachine()
    {
        base.EnemyStateMachine();
    }

    public override void EnemyPatrol()
    {
        base.EnemyPatrol();
        
    }

    public override void EnemyBattle()
    {
        base.EnemyBattle();
    }

    public override void EnemyDie()
    {
        if(!_isDie)
        {
            _isDie = true;

            _enemyAnimator.SetTrigger("Die");
            SetDie();
            StartCoroutine(DieAnimation());
        }

        IEnumerator DieAnimation()
        {
            yield return null;

            int stateHash = Animator.StringToHash("Die");

            yield return new WaitUntil(() => CheckDieAnim());
            yield return new WaitForSeconds(0.5f);

            Destroy(this.gameObject);

            bool CheckDieAnim()
            {
                AnimatorStateInfo stateInfo = _enemyAnimator.GetCurrentAnimatorStateInfo(0);
                float animTime = stateInfo.normalizedTime;

                if (stateInfo.shortNameHash == stateHash)
                {
                    if (animTime >= 1.0f) return true;
                }

                return false;
            }
        }

        void SetDie()
        {
            EnemyPathReset();
            this.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    protected override void AttackPattern()                // 파티클 및 애니메이션 관리
    {
        string Attack = "Attack";
        string Under = "_";
        string Delay = "Delay";

        bool firstCheck = false;
        bool secondCheck = false;

        bool _isOnStun = false;

        StartCoroutine(MaseAttackPattern());

        IEnumerator MaseAttackPattern()
        {
            _isPlayingAttackAnim = true;
            //_attackIndex = Random.Range(0, 4);       // 0 1 2 Mase는 최대 3
            _attackIndex = 3;
            _enemyAnimator.SetInteger("AttackNum", _attackIndex);

            int stateHash1 = Animator.StringToHash($"{Attack}{Under}{_attackIndex}");
            int stateHash2 = Animator.StringToHash($"{Attack}{Delay}");

            AttackPassHash(stateHash1, switchAnimTime());

            yield return new WaitUntil(() => AnimCheck());
            yield return new WaitUntil(() => InDelaytCheck());

            float waitDelayTime = (_isOnStun) ? 0.1f : 2f;

            yield return new WaitForSeconds(waitDelayTime);

            if (_enemyFiledView._playerTransform != null) EnemyPathResum(_enemyFiledView._playerTransform.position);        // 탐지 범위 밖으로 나가지 않았을 경우만

            _isPlayingAttackAnim = false;
            _isAttack = false;

            bool AnimCheck()
            {
                AnimatorStateInfo stateinfo = _enemyAnimator.GetCurrentAnimatorStateInfo(0);
                float animTime = stateinfo.normalizedTime;

                if (stateinfo.shortNameHash == stateHash1) firstCheck = true;
                //Debug.Log($"{firstCheck}첫 체크");
                return firstCheck;
            }

            bool InDelaytCheck()
            {
                if(!_isOnStun) _isOnStun = (_isStun);

                AnimatorStateInfo stateinfo = _enemyAnimator.GetCurrentAnimatorStateInfo(0);
                float animTime = stateinfo.normalizedTime;

                if (stateinfo.shortNameHash == stateHash2) secondCheck = true;

                return secondCheck;
            }

        }

        float switchAnimTime()
        {
            return _attackIndex switch
            {
                3 => 0.65f,                 // 얘는 이게 패링임
                2 => 0.25f,
                _ => 0.55f
            };
        }
        
    }

    
    /// <summary>
    /// EnemyAttack에 전달, 애니메이션 hash와 각 몬스터 패턴 시 때릴 타이밍 시간 전달
    /// </summary>
    /// <param name="stateHash"></param>
    /// <param name="attackTime"></param>
    public void AttackPassHash(int stateHash, float attackTime)
    {
        StartCoroutine(_enemyAttack.ReceiveHash(stateHash,_attackIndex, attackTime));
    }

    public void LateUpdate()
    {
        _enemyAnimator.SetBool("Idle", _isPIdle);
        _enemyAnimator.SetBool("Patrol", _isPatrol);
        _enemyAnimator.SetBool("Battle", _isBattle);
        _enemyAnimator.SetBool("Run",_isRun);
        _enemyAnimator.SetBool("Attack", _isAttack);
    }
}
