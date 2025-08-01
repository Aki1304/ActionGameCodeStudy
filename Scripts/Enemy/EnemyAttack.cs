using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public enum EnemyAttackState
{
    Pan, Square, Circle
}

/// <summary>
/// Enemy 의 패턴 실 공격및 파티클 인디케이터 관리
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("해당 몬스터의 Indicate MeshRender")]
    [SerializeField] private MeshRenderer[] _thisMeshRender;

    private List<MaterialPropertyBlock> _InsMesh = new List<MaterialPropertyBlock>();

    [Header("해당 몬스터의 Indicate Object 관리")]
    [SerializeField] public GameObject[] _thisIndicator;

    [Header("해당 몬스터의 Particle 관리")]
    [SerializeField] public ParticleSystem[] _particleSystem;

    [Header("몬스터의 패링 이펙트")]
    [SerializeField] public ParticleSystem _parryingParticle;

    [Header("총 공격에 쓰는 state 배열")]
    [SerializeField] private EnemyAttackState[] _enemyArrayStates;

    [Header("콜라이더 Object")]
    [SerializeField] private GameObject _enemyColObject;

    private Enemy _enemy;
    private Animator _animator;
    private EnemyUI _enemyUI;

    private int _curIdxNum;

    public bool _enemyParried = false;
    private bool _isStunWait =false;

    #region 프로퍼티
    private Player _player { get { return PlayerManager.Instance.GetPlayer; } }
    #endregion

    public void Start()
    {
        // 객체 정보
        _enemy = GetComponent<Enemy>();
        _enemyUI = GetComponent<EnemyUI>();
        _animator = GetComponent<Animator>();

        // 메쉬랜더러 block으로 프로퍼티 변형 시 원형은 유지
        foreach (var renderer in _thisMeshRender)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            _InsMesh.Add(propBlock);
        }

        // 현재 안 쓰는 Collider의 연산을 막기 위해 컴포넌트 끄기
        foreach(var component in _enemyColObject.GetComponents<Collider>()) component.enabled = false;
    }

    public void SetIndicateRange(float var)
    {
        _InsMesh[_curIdxNum].SetFloat("_Range", var);
        _thisMeshRender[_curIdxNum].SetPropertyBlock(_InsMesh[_curIdxNum]);
    }


    public IEnumerator ReceiveHash(int stateHash, int idxNum, float attackTime)
    {
        AnimatorStateInfo stateinfo;
        _curIdxNum = idxNum;

        bool End = false;
        bool firstCheck = false;

        bool Check()
        {
            if (_enemy.IsCheckStop()) return true;

            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateinfo.shortNameHash == stateHash;
        }

        yield return new WaitUntil(() => Check());

        _thisIndicator[_curIdxNum].SetActive(true);

        while (true)
        {
            if (_enemy.IsCheckStop()) break;

            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);

            float animTime = stateinfo.normalizedTime;
            float progress = Mathf.Clamp01(animTime / attackTime);

            SetIndicateRange(progress);
            EnemyEffectDiff(progress);

            if (End) break;

            yield return null; // 프레임 단위로 반응
        }

        _thisIndicator[_curIdxNum].SetActive(false);

        
        void EnemyEffectDiff(float progress)                        // 각 캐릭터마다 다른 특수기 넣고 싶다면 작성
        {
            switch (_enemy._monster)
            {
                case Enemy.Monster:
                    {
                        if (idxNum == 3)
                        {
                            if(progress >= 0.9f && !firstCheck)
                            {
                                firstCheck = true;
                                Invoke("StateCollider",1f);
                                UseParticle(idxNum);
                                End = true;
                                break;
                            }
                        }
                        else ReturnDefault(); break;

                    }
            }

            void ReturnDefault()
            {
                if (progress >= 1.0f)
                {
                    AttackEffect();
                    StartCoroutine(ReturnParticle());
                    End = true;
                }
            }
        }

    }


    /// <summary>
    /// 실 타격 콜라이더 설정 및 파티클 설정
    /// index 참조
    /// </summary>
    public void AttackEffect()
    {
        StateCollider();
        UseParticle(_curIdxNum);
    }

    private void StateCollider()
    {
        EnemyAttackState state = _enemyArrayStates[_curIdxNum];

        // 가져갈 인디케이터의 사이즈 와 포즈
        Vector3 size = _thisIndicator[_curIdxNum].transform.localScale;
        Vector3 pos = _thisIndicator[_curIdxNum].transform.localPosition;

        switch (state)
        {
            case EnemyAttackState.Pan:
                {
                    float panAngle = _thisMeshRender[_curIdxNum].material.GetFloat("_Angle"); ;
                    float radius = transform.localScale.x * size.x * 0.5f;                  // 현재 부모 오브젝트 스케일 값도 곱해줘야함... 로컬 스케일 곱해주기 

                    Collider[] hitCols = Physics.OverlapSphere(transform.position, radius);

                    //Debug.Log($"각도 크기{panAngle} radius {radius}");

                    foreach(var target in hitCols)
                    {
                        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                        float angle = Vector3.Angle(transform.forward, dirToTarget);

                        if (angle <= panAngle / 2f)
                        {
                            if(target.CompareTag("Player"))
                            {
                                // 가져온 타겟을 적중 시킨다. 매니저에서 이미 Player의 메모리를 갖고 있음.
                                // 그래서 조건만 만족하면 그냥 매니저에서 바로 타격 지시
                                //Debug.Log("부채꼴 함");
                                _player.TakeDamage(_enemy.GetStats.atk);
                                break;
                            }
                        }
                    }

                    break;
                }
            case EnemyAttackState.Square:
                {
                    BoxCollider col = _enemyColObject.GetComponent<BoxCollider>();

                    col.center = pos;
                    col.size = new Vector3(size.x, size.z, size.y);

                    col.enabled = true;
                    break;
                }
            case EnemyAttackState.Circle:
                {
                    SphereCollider col = _enemyColObject.GetComponent<SphereCollider>();
                    
                    col.center = pos;
                    col.radius = size.x * 0.5f;     // radius 는 반지름...

                    col.enabled = true;
                    break;
                }
            default: break;
        }

      
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            // 안에 플레이어가 있다면
            Debug.Log("타격 성공");
            _player.TakeDamage(_enemy.GetStats.atk);
            DisableComponent();
        }
        else
        { DisableComponent(); Debug.Log("공격 실패"); }


        void DisableComponent()
        {
            // 현재 안 쓰는 Collider의 연산을 막기 위해 컴포넌트 끄기
            foreach (var component in _enemyColObject.GetComponents<Collider>())
            {
                if (component.enabled)
                {
                    component.enabled = false;
                }

            }
        }

    }


    private void ParryingParticle()
    {
        _parryingParticle.Clear();
        _parryingParticle.Play();

        StartCoroutine(ParryingAction());

        IEnumerator ParryingAction()
        {
            float startTime = Time.time;
            float distancetime = 0.6f;

            while(Time.time - startTime <= distancetime)
            {
                //Debug.Log($"체킹{_enemyParried}패링상태 {_enemy._isStun}스턴 상태");
                if(!_enemyParried) _enemyParried = true;
                if (_enemy._isStun) { _enemyParried = false; break; }
                yield return null;
            }

            _enemyParried = false;


            if (_enemy._isStun)                  // 스턴일 경우
            {
                yield return new WaitUntil(() => _isStunWait);
                yield return StartCoroutine(_enemyUI.StunUIEnemy(5f));
                _animator.speed = 1f;
                _enemy._isStun = false;
                _isStunWait = false;

            }
        }
    }

    public void SetSuccessParrying()
    {
        _enemy._isStun = true;               // 스턴 상태로 만듦.

        Vector3 playerPos = PlayerManager.Instance.GetPlayer.currentPos;
        transform.LookAt(playerPos);

        _animator.SetTrigger("Stun");
    }

    private void StunDelay()
    {
        _animator.speed = 0f;
        _isStunWait = true;
    }

    private void UseParticle(int idxNum)
    {
        _particleSystem[idxNum].Clear();
        _particleSystem[idxNum].Play();
    }
    public void StopParticle(int idxNum)
    {
        _particleSystem[idxNum].Stop();
    }

    private IEnumerator ReturnParticle()
    {
        yield return new WaitUntil(() => _particleSystem[_curIdxNum].isStopped);
        //Debug.Log("파티클 끝남");
        StopParticle(_curIdxNum);
    }
}
