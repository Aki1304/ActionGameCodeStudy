using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Character : MonoBehaviour
{
    #region 플레이어 컴포넌트
    [Header("플레이어 관련 컴포넌트")]
    protected Rigidbody rigidBody;

    private Vector3 moveMent;

    [Space(10f)]
    [Header("카메라 관련 헤더")]
    [SerializeField, Tooltip("카메라")] public CameraMove cameraMove;

    [Space(10f)]
    [Header("애니메이션 관련 체크")]
    // 애니메이션 불 값

    [SerializeField, Tooltip("캐릭터 클래스")] protected string _isCharName;
    [SerializeField] public float _isSpeed = 0f;
    [SerializeField] public bool _isRun = false;
    [SerializeField, Tooltip("AnyState 불")] public bool _anyState = false;
    [SerializeField, Tooltip("SkillAnim 불")] protected bool _skillUsing = false;

    [Header("기본 공격 변수")]
    [SerializeField, Tooltip("기본 공격 불 값 체크")] public bool _normalAttackBool = false;
    [SerializeField, Tooltip("기본 공격 애니메이션 도중 눌렀는지 체크")] public bool _normalAttackReClick = false;

    #endregion

    #region 플레이어관련 객체 생성
    // 스크립트
    protected CharacterStatus _status;
    protected PlayerAnimState _playerAnimState;
    protected CharacterHP _characterHP;
    protected PlayerAnimator _playerAnimator;
    protected ClickAttackDelay _clickAttackDelay;
    protected PlayerBuffBuffer _buffBuffer;

    // 플레이어 UI 관련
    protected PlayerUI _playerUI;

    // 스킬 스크립트
    protected PlayerSkill _playerSkill;

    //  컴포넌트
    protected Animator _animator;

    // 퀵슬룻
    protected QuickSlot _quickSlot;

    // 구르기
    protected Evade _evade;

    // 패링
    private Parrying _parrying;
    #endregion

    #region 플레이어 프로퍼티
    // 현재 캐릭터 스탯
    public CharacterStatus charStats { get { return _status; } }

    // 현재 버퍼 참고
    public PlayerBuffBuffer GetBuffBuffer { get { return _buffBuffer; } }

    // 패링 리턴 참고
    public bool ReturnCamParrying { get { return _parrying._returnCamParrying; } }

    // 패링 성공했는지 변화참고
    public bool GetOnParrying { get { return _parrying.OnParrying; } }

    // 현재 캐릭터 위치
    public Vector3 currentPos { get { return this.transform.position; } }

    // 플레이어 애니메이터
    public Animator GetAnimator { get { return _animator; } set { _animator = value; } }

    // 플레이어 애니 스테이트 체크
    public void SetInputAnyState(bool active) { _anyState = active; }

    // 플레이어 isSpeed 0으로 변경
    public void SetStop() { _isSpeed = 0f; moveMent = Vector3.zero; }

    // 플레이어가 InputMovement하는지 체크
    public bool IsMoving { get { return moveMent.sqrMagnitude > 0.01f; } }

    // 플레이어가 스킬을 쓰고 있는지 판별
    public bool GetSkillUsing { get { return _skillUsing; } set { _skillUsing = value; } }

    // 플레이가 기본 공격을 하고 있는지
    public bool GetNormalAttackBool { get { return _normalAttackBool; } }

    // 플레이어 구르기
    public bool GetEvade { get { return _evade._evade; } }

    // 플레이어 맞음
    public bool GetHit { get; set; }

    // 플레이어가 행동 전 체크
    public bool ReturnFirstCheck()
    {
        if (_anyState || _normalAttackBool || _skillUsing || !KeySet.GetMouseIsLock) return true;
        return false;
    }

    // 스태미나 체크
    public bool ReturnCheckStamina(float skillSta) => (GetCharHP.currentStamina >= skillSta);
    public CharacterHP GetCharHP { get { return _characterHP; } }
    #endregion

    #region 호출 프로퍼티
    protected GameManager GM { get { return GameManager.Instance; } }
    protected KeySetting KeySet {  get { return GM.KeySet; } }
    public PlayerAnimState GetAnimState { get { return _playerAnimState; } }

    #endregion

    public virtual void Awake()
    {
        InitClass();
    }

    public virtual void Start()
    {
        CharInit();
    }
    public virtual void Update()
    {
        CharInputMoveKey();
        KeyInput();
        _clickAttackDelay.DelayForClick();
    }

    public virtual void FixedUpdate()
    {
        CharMove();
    }

    public virtual void LateUpdate()
    {
        CamCheckAndMove();
    }

    private void InitClass()
    {
        // 객체 생성
        _status = new CharacterStatus();
        _playerAnimState = new PlayerAnimState();
        _characterHP = new CharacterHP();
        _clickAttackDelay = new ClickAttackDelay();

        _playerUI = UIManager.Instance.GetPlayerUI;                 // Ui 매니저 참조
    }
    private void CharInit()
    {
        // 컴포넌트 적용
        rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _playerSkill = GetComponent<PlayerSkill>();
        _quickSlot = GetComponent<QuickSlot>();
        _evade = GetComponent<Evade>();
        _parrying = GetComponent<Parrying>();
        _buffBuffer = GetComponent<PlayerBuffBuffer>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        // 플레이어 애니메이션 초기화
        _playerAnimState.SetState(PlayerAnimState.AnimState.Idle);

        // 플레이어 UI 초기화
        _playerUI.UICharacterInit();

        // 변수 초기화
        SetInputAnyState(false);

        // 스탯 초기화
        CharStatusSetting();
    }


    protected void CharInputMoveKey()
    {
        if(!KeySet.GetMouseIsLock) { SetStop(); }

        moveMent.x = Input.GetAxisRaw("Horizontal");
        moveMent.z = Input.GetAxisRaw("Vertical");
        moveMent.y = 0;                                 // Y축 움직임은 필요 없으므로 항상 0으로 고정 점프 X

        InputState();
    }

    private void KeyInput()
    {
        if(!AttackCheck() && _clickAttackDelay.CanAttackAfterMoveDelay())
        {
            if (Input.GetMouseButtonDown(0))
            {
                CharNormalAttack();                         // abstract 추상화로 필수 기능 작성 요구
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift)) SetInputRun(); // 달리기 Input

        void SetInputRun()                                      // 달리기 Anim Bool 관리
        {
            _isRun = !_isRun;

            _playerUI.GetWRSprite = _playerUI.GetReturnWRSprite(_isRun);
        }

        bool AttackCheck()
        {
            return _skillUsing || _anyState || !KeySet.GetMouseIsLock || GetHit;
        }

    }

    private void InputState()       // 각 캐릭터의 움직이는 상태 값 변경
    {
        if (ReturnFirstCheck())
        {
            moveMent = Vector3.zero;
            return;
        }

        if (KeySet.MoveKeys.Any(Input.GetKey))                          // wasd아무거나 누르고 있다면
        {
            if (!_isRun)
            { _playerAnimState.SetState(PlayerAnimState.AnimState.Walk); _isSpeed = _status._walkSpeed; }
            else { _playerAnimState.SetState(PlayerAnimState.AnimState.Run); _isSpeed = _status._runSpeed; }
        }
        else                                                               // 움직이지 않으면
        {
            _playerAnimState.SetState(PlayerAnimState.AnimState.Idle);
            _isSpeed = 0f;
        }
    }

    protected void CharMove()      // 각 캐릭터의 움직임
    {
        // 이동 속도 설정
        float moveSpeed = GetSpeed();

        // 카메라 기준 이동 방향 계산
        Vector3 camForward = cameraMove.GetCameraObject().transform.forward; // 카메라의 전방 벡터
        Vector3 camRight = cameraMove.GetCameraObject().transform.right;     // 카메라의 오른쪽 벡터
        camForward.y = 0f; // Y축 제거 (수평 이동만)
        camRight.y = 0f;

        Vector3 moveDir = (camForward.normalized * moveMent.z + camRight.normalized * moveMent.x).normalized
                          * moveSpeed * Time.fixedDeltaTime;

        // 이동
        rigidBody.MovePosition(rigidBody.position + moveDir);

        // 회전
        if (IsMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir); // 이동 방향 기준 회전
            rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }

        float GetSpeed()
        {
            switch (_playerAnimState.GetPlayerState)
            {
                case PlayerAnimState.AnimState.Walk:
                    return 3f;
                case PlayerAnimState.AnimState.Run:
                    return 6f;
                default: return 0f;
            }
        }
    }
    protected void CamCheckAndMove()
    {
        cameraMove.CameraRotate(this.gameObject.transform, currentPos);
    }
    protected abstract void CharNormalAttack();

    protected IEnumerator DelayForCode(float time)
    {
        float current = 0f;
        float percent = 0f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            yield return null;
        }
    }

    protected abstract void CharStatusSetting();

    /// <summary>
    /// StatsNumber 0 => 체력 1 => 스태미나
    /// </summary>
    /// <param name="StatsNumer"></param>
    /// <param name="percentEffect"></param>
    //public void SetStatusChange(int StatsNumer, float percentEffect = 0f)
    //{
    //    switch (StatsNumer)
    //    {
    //        case 0:
    //            _characterHP.CharHPChange((int)percentEffect);
    //            break;
    //        case 1:
    //            _characterHP.CharStaminaChange((int)percentEffect);
    //            break;
    //        default:
    //            Debug.Log("X");
    //            break;
    //    }
    //}

    public void GetHeal(int heal)
    {
        _characterHP.GetHeal(heal);
        _playerUI.PassHealInfo();
    }

    public void TakeDamage(int damage)
    {
        if (GetEvade || GetOnParrying) return;

        _playerAnimator.AnyStateCheckAnim(PlayerAnimState.AnyState.Hit);
        _characterHP.TakeDamage(damage);
        _playerUI.PassDamageInfo();
    }

    public void UseStamina(int stamina)
    {
        _characterHP.UseStamina(stamina);
        _playerUI.PassStaminaInfo();
    }

    public void UseSkill(int number)
    {
        if(!ReturnFirstCheck())                         // 다른 상태가 아닌지 체크 
        {
            _playerSkill.GetUseSkill(_animator, number);
        }
    }

    public abstract void AttackRaycast(int number);

    public void GetRotateEnemy()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 2.8f);

        foreach (var col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))                         // 가장 먼저 바라본 놈 
            {
                Vector3 enemyPos = col.transform.position;
                transform.LookAt(enemyPos);
                break;
            }
        }
    }
}
