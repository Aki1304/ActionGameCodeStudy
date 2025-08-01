using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Parrying : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particleSuccess = new ParticleSystem[2];

    private Animator _animator;
    private Character _character;


    private bool _isParrying = false;
    private bool _onParrying = false;
    [HideInInspector] public bool _returnCamParrying = false;

    #region 프로퍼티
    private PlayerManager PM { get { return PlayerManager.Instance; } }
    private bool UsingSkill { get { return _character.GetSkillUsing; } set { _character.GetSkillUsing = value; } }
    public bool OnParrying { get { return _onParrying; } }
    public KeySetting GetKeySet { get { return GameManager.Instance.KeySet; } }
    #endregion

    private void FixedUpdate()
    {
        RangeParring();
    }



    void ParryingInput()
    {
        if (Input.GetKeyDown(GetKeySet._parryingKey))
        {
            if (CheckClick()) return;
            ParryingUse();
        }

        bool CheckClick()
        {
            return !GetKeySet.GetMouseIsLock || _character.GetSkillUsing ||
                _character.GetHit;
        }
    }

    bool CheckPossibleUse()
    {
        // 스태미나로 사용여부 체크
        return _character.ReturnCheckStamina(PM.GetSkillData[0].SkillStaminaUse);
    }

    void ParryingUse()
    {
        if (!CheckPossibleUse()) { Debug.Log("스태미나 false"); return; }

        string parName = PM.GetSkillData[0].SkillName;
        string Under = "_";
        string Idle = "Idle";

        int stateHash1 = Animator.StringToHash($"{parName}");
        int stateHash2 = Animator.StringToHash($"{parName}{Under}{Idle}");

        if (!UsingSkill)
        {
            SetParrying();
        }

        void SetParrying()
        {
            Debug.Log("패링");
            UsingSkill = true;
            SetSkill();
            StartCoroutine(SkillAnim());
        }

        void SetSkill()
        {
            _animator.SetBool(parName, true);
            _character.UseStamina((int)PM.GetSkillData[0].SkillStaminaUse);
            _character.SetStop();
        }

        IEnumerator SkillAnim()
        {
            yield return null;

            yield return new WaitUntil(() => AnimCheck());
            yield return new WaitUntil(() => TimeCheck());

            _animator.SetBool(parName, false);
            UsingSkill = false;

            bool AnimCheck()
            {
                if (_character.GetHit || OnParrying) return true;

                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
                return (stateInfo.shortNameHash == stateHash1);
            }

            bool TimeCheck()
            {
                if (_character.GetHit) return true;

                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(1);
                float animTime = stateInfo.normalizedTime;

                return (stateInfo.shortNameHash == stateHash2) && animTime >= 1.0f;
            }
        }
    }

    void ParryingInit()
    {
        _character = GetComponent<Character>();
        _animator = GetComponent<Animator>();
    }

    public void EnableParried() => _isParrying = true;
    public void DisableParried() { if (_onParrying) return; _isParrying = false; }
    public void ReturnTick() =>_returnCamParrying = true;

    public void ParticleUse()
    {
        foreach(ParticleSystem ps in _particleSuccess)
        {
            ps.Clear(); ps.Play();
        }
    }

    private void Start()
    {
        ParryingInit();
    }

    // Update is called once per frame
    private void Update()
    {
        ParryingInput();
    }

    private void RangeParring()
    {
        if (!_isParrying) return;

        float range = 1.25f;
        _returnCamParrying = false;

        Collider[] hitCols = Physics.OverlapSphere(this.transform.position, range);

        foreach (Collider col in hitCols)
        {
            Vector3 dirToTarget = (col.transform.position - this.transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if(angle <= 60)
            {
                if (col.CompareTag("Enemy"))
                {
                    EnemyAttack _enemyAttack = col.gameObject.GetComponent<EnemyAttack>();
                    Enemy _enemy = col.gameObject.GetComponent<Enemy>();
                    if (!_enemyAttack._enemyParried) return;

                    _onParrying = true;
                    Vector3 enemyPos = col.gameObject.transform.position;
                    transform.LookAt(enemyPos);                                   // 바라보게 만들기
                    _enemyAttack.SetSuccessParrying();                            // 적 패링 성공 > 그로기 진입
                    StartCoroutine(EffectParrying(enemyPos));                     // 카메라 무빙 시점 효과 지정 등
                    StartCoroutine(TakeEnemyParryingDamagea(_enemy));             // 데미지 주는 시점 계산
                    break;
                }
            }
        }

    }

    private IEnumerator TakeEnemyParryingDamagea(Enemy enemy)
    {
        yield return new WaitUntil(() => _returnCamParrying);
        enemy.InvokeDamage();
    }

    private IEnumerator EffectParrying(Vector3 enemyPos)
    {
        _animator.speed = 0f;
        int randIndex = Random.Range(0,2);

        ParticleUse();                                                // 파티클 사용

        // 플레이어와 사이를 구해서 멀어지게 하기
        float duration = 0.35f;

        Vector3 startPos = this.transform.position;
        Vector3 dir = (this.transform.position - enemyPos).normalized;          // 뒤로 밀리는 방향 (enemyPos에서 반대 방향)
        Vector3 targetPos = enemyPos + dir * 2f;

        float elapsed = 0f;
        bool _isMovingCam = false;

        while (elapsed < duration)
        {
            if(elapsed >= 0.1f && !_isMovingCam)
            {
                _isMovingCam = true;
                StartCoroutine(_character.cameraMove.CamAction(enemyPos,randIndex));
            }

            if (elapsed >= 0.25f) _animator.speed = 1f;
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0~1까지의 진행도

            // 선형 보간으로 부드럽게 이동
            this.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 정확하게 목표 위치로 맞춤
        this.transform.position = targetPos;
        _animator.SetTrigger("O_Parrying");
        _isParrying = false;
        _onParrying = false;
    }

    

}
