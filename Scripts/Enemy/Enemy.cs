using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyStats
{
    public string name { get; private set; }
    public int maxHp { get; private set; }
    public int curHp { get; private set; }
    public int atk { get; private set; }
    public int def { get; private set; }
    public float moveSpeed { get; private set; }
    public float runSpeed { get; private set; }

    public EnemyStats (string name, int hp, int atk, int def, float moveSpeed, float runSpeed)
    {
        this.name = name;
        this.maxHp = hp;
        this.curHp = hp;
        this.atk = atk;
        this.def = def;
        this.moveSpeed = moveSpeed;
        this.runSpeed = runSpeed;
    }

    public void DamageToHP(int damage)
    {
        if ((curHp - damage) <= 0) curHp = 0;
        else curHp -= damage;
    }
}

public abstract class Enemy : MonoBehaviour
{

    public enum Monster
    {
        Mase
    }

    public enum EnemyState
    {
        Patrol,
        Battle,
        Die
    }

    protected EnemyStats _enemyStats;
    protected Animator _enemyAnimator;
    protected NavMeshAgent _navAgent;
    protected EnemyAttack _enemyAttack;
    protected DamageFormula _damageFormula;
    protected EnemyUI _enemyUI;

    [Header("해당 몬스터의 탐지 범위")]
    [SerializeField] protected EnemyFiledView _enemyFiledView;

    [Header("해당 몬스터")]
    [SerializeField] public Monster _monster;

    [Header("몬스터 상태")]
    [SerializeField] protected EnemyState _enemyState;
    public EnemyState SetState { get { return _enemyState; } set { _enemyState = value; } }

    [Header("몬스터 전용 Navmesh")]
    [SerializeField] protected NavMeshSurface _nevSurface;

    [Header("몬스터 패트롤 속도 조절")]
    [SerializeField] private float _stopTime;


    [Header("몬스터 멈추는 거리")]
    [SerializeField] private float _stopDistance;

    private float _delayTime;                    // delayTime에 따른 체크 조건

    #region 프로퍼티 호출
    public EnemyStats GetStats { get { return _enemyStats; } }

    public bool IsDie { get { return GetStats.curHp <= 0; } }
    #endregion 

    #region AI 호출
    public void EnemyPathReset()
    {
        _navAgent.ResetPath();
        _navAgent.isStopped = true;
    }

    public void EnemyPathResum(Vector3 pos)
    {
        _navAgent.isStopped = false;
        _navAgent.SetDestination(pos);
    }

    public bool PossibleChangeState()
    {
        return _isAttack || _isPlayingAttackAnim || _isStun;
    }
    #endregion
    #region 상태 체크 및 애니메이션 setbool값
    [Header("애니메이션 및 행동 체크")]
    [SerializeField] protected bool _isPIdle = false;
    [SerializeField] protected bool _isPatrol = false;
    [SerializeField] protected bool _isBattle = false;
    [SerializeField] protected bool _isRun = false;
    [SerializeField] public bool _isStun = false;
    [SerializeField] protected bool _isDie = false;
    [SerializeField] protected bool _isAttack = false;
    [SerializeField] protected bool _isPlayingAttackAnim = false;
    [SerializeField] protected int _attackIndex;
    #endregion

    #region 상태 호출
    public bool IsCheckStop() => _isDie || _isStun;
    #endregion 

    private void Start()
    {
        _stopTime = Random.Range(2, 6);     // 랜덤하게 패트롤 하기 전 시간 설정

        // 패트롤 시간 기초
        _delayTime = _stopTime;
    }

    private void Awake()
    {
        EnemyInit();
    }

    public virtual void Update()
    {
        EnemyStateMachine();
        CheckHP();

        void CheckHP()
        {
            if (_enemyStats.curHp <= 0)
            {
                _enemyState = EnemyState.Die;
            }
        }
    }

    /// <summary>
    /// base로 받고 각 적의 속성값 수치 넣어주기
    /// </summary>
    public virtual void EnemyInit()
    {
        // 컴포넌트
        _enemyAnimator = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _enemyAttack = GetComponent<EnemyAttack>();
        _damageFormula = new DamageFormula();
        _enemyUI = GetComponent<EnemyUI>();

        //_navAgent.updatePosition = true;
        //_navAgent.updateRotation = true;
    }

    public virtual void EnemyStateMachine()
    {
        switch(_enemyState)
        {
            case EnemyState.Patrol:
                {
                    EnemyPatrol();
                    break;
                }
            case EnemyState.Battle:
                {
                    EnemyBattle();
                    break;
                }
            case EnemyState.Die:
                {
                    EnemyDie();
                    break;
                }
            default:
                break;
        }
    }

    public virtual void EnemyPatrol()
    {
        if (PossibleChangeState()) return;

        SetFirstChange();
        SetPatrolPos();
        WorkingPatrol();

        void SetFirstChange()
        {
            if (!_isPatrol)
            {
                _isBattle = false;
                _isRun = false;
                _isPatrol = false;
                _isPIdle = true;

                // agent속성 변경
                _navAgent.stoppingDistance = 0f;            // 패트롤일땐 무조건0
                _navAgent.speed = _enemyStats.moveSpeed;
            }
        }

        void SetPatrolPos()
        {
            if (_isPatrol) return;

            _delayTime += Time.deltaTime;

            if (_delayTime >= _stopTime)
            {
                _isPIdle = false;
                Vector3 randomPos = GetRandomPositionOnNavMesh();           // 아래 랜덤하게 좌표 찍기 실행
                EnemyPathResum(randomPos);                        // 에너미에게 좌표 목표 전달
                _isPatrol = true;
            }

        }

        void WorkingPatrol()
        {
            if (!_isPatrol) return;

            // 적이 경로계산 중이 아니라면 false 에너미의 남은 거리가 0.1미만이며 true 경로 Path가 존재하지 않는다면 true일 경우
            // 즉 경로에 도착을 했을 경우만 기존 경로는 부정확한 경우가 간혹 존재해서 세세한 조건으로 변경
            if (!_navAgent.pathPending && _navAgent.remainingDistance <= 0.1f && !_navAgent.hasPath)
            {
                _delayTime = 0f;
                _stopTime = Random.Range(4, 7);
                _isPIdle = true;
                _isPatrol = false;
            }
        }

        Vector3 GetRandomPositionOnNavMesh()
        {
            Vector3 cenor = Vector3.zero;
            float radius = 15f;

            Vector3 randomDirection = cenor + Random.insideUnitSphere * radius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.position; // NavMesh가 없는 경우 현재 위치 유지
        }
    }

    public virtual void EnemyBattle()
    {
        SetFirstChange();
        WorkingBattle();

        void SetFirstChange()
        {
            if(!_isBattle)
            {
                _isBattle = true;
                _isPatrol = false;
                _isPIdle = false;


                // agent속성 변경
                _navAgent.stoppingDistance = _stopDistance;
                _navAgent.speed = _enemyStats.runSpeed;

            }
        }

        void WorkingBattle()
        {
            if (!_isBattle) return;
            if (PossibleChangeState()) return;

            if (ArriveTargetPath() && _navAgent.velocity.sqrMagnitude == 0f)
            {
                if (RotateLookPlayer()) return;    // 회전 중이면 여기서 return

                TryAttack();
            }
            else
            {
                ResumePath();
            }


            bool ArriveTargetPath()
            {
                const float arrivalTolerance = 0.1f;
                return _navAgent.remainingDistance - _navAgent.stoppingDistance <= arrivalTolerance;
            }

            bool RotateLookPlayer()
            {
                if (_enemyFiledView._playerTransform == null) return false;

                Transform player = _enemyFiledView._playerTransform;
                Vector3 dir = player.position - transform.position;

                if (dir.sqrMagnitude < 0.001f) return false;

                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 180f * Time.deltaTime);

                float angle = Quaternion.Angle(transform.rotation, targetRot);

                Debug.Log("추적중");

                return angle > 0.1f;    // true면 아직 회전 중
            }

            void TryAttack()
            {
                if (_isAttack) return;

                _isAttack = true;
                _isRun = false;
                AttackPattern();    // 각 몬스터마다 패턴 추상화 시키기
                                    // Debug.Log("공격 애니 진입");
            }

            void ResumePath()
            {
                if (_enemyFiledView._playerTransform != null)
                {
                    EnemyPathResum(_enemyFiledView._playerTransform.position);
                }

                if (!_isRun) _isRun = true;
            }
        }
    }

    protected abstract void AttackPattern();

    public abstract void EnemyDie();

    #region 적 데미지 받는 공식 및 애니메이션
    public void TakeDamage()
    {
        bool ReturnCritical = _damageFormula.CheckCritical();
        int GetDamage = _damageFormula.EnemyTakeDamage(_enemyStats.def);

        if( ReturnCritical )
        {
            Invoke("InvokeDamage",0.15f);
        }
        //Debug.Log($"데미지 {GetDamage} 현재 적 체력 {_enemyStats.curHp}");
        HealthUpdate(GetDamage);
    }

    public void InvokeDamage()
    {
        int GetDamage = _damageFormula.EnemyTakeDamage(_enemyStats.def, true);
        //Debug.Log($"크리티컬 발동! 데미지 {GetDamage} 현재 적 체력 {_enemyStats.curHp}");
        HealthUpdate(GetDamage, true);
    }

    public void HealthUpdate(int damage, bool criticalCheck = false)  
    {
        if (IsDie) return;

        _enemyStats.DamageToHP(damage);
        _enemyUI.UITakeDamge();
        FontEffectUse(damage, criticalCheck);
    }

    private void FontEffectUse(int damage, bool criticalCheck = false)
    {
        GameObject font = UIManager.Instance.GetDMGPoolUI.ReturnObjectPopPool();
        DamageFontEffect fontEffect = font.GetComponent<DamageFontEffect>();

        fontEffect.SetActiveTrue(damage, this.transform.position, criticalCheck);
    }

    public void PerkDamage(int damage)
    {
        HealthUpdate(damage);
    }

    #endregion
}
