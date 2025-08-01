using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Character : MonoBehaviour
{
    #region �÷��̾� ������Ʈ
    [Header("�÷��̾� ���� ������Ʈ")]
    protected Rigidbody rigidBody;

    private Vector3 moveMent;

    [Space(10f)]
    [Header("ī�޶� ���� ���")]
    [SerializeField, Tooltip("ī�޶�")] public CameraMove cameraMove;

    [Space(10f)]
    [Header("�ִϸ��̼� ���� üũ")]
    // �ִϸ��̼� �� ��

    [SerializeField, Tooltip("ĳ���� Ŭ����")] protected string _isCharName;
    [SerializeField] public float _isSpeed = 0f;
    [SerializeField] public bool _isRun = false;
    [SerializeField, Tooltip("AnyState ��")] public bool _anyState = false;
    [SerializeField, Tooltip("SkillAnim ��")] protected bool _skillUsing = false;

    [Header("�⺻ ���� ����")]
    [SerializeField, Tooltip("�⺻ ���� �� �� üũ")] public bool _normalAttackBool = false;
    [SerializeField, Tooltip("�⺻ ���� �ִϸ��̼� ���� �������� üũ")] public bool _normalAttackReClick = false;

    #endregion

    #region �÷��̾���� ��ü ����
    // ��ũ��Ʈ
    protected CharacterStatus _status;
    protected PlayerAnimState _playerAnimState;
    protected CharacterHP _characterHP;
    protected PlayerAnimator _playerAnimator;
    protected ClickAttackDelay _clickAttackDelay;
    protected PlayerBuffBuffer _buffBuffer;

    // �÷��̾� UI ����
    protected PlayerUI _playerUI;

    // ��ų ��ũ��Ʈ
    protected PlayerSkill _playerSkill;

    //  ������Ʈ
    protected Animator _animator;

    // ������
    protected QuickSlot _quickSlot;

    // ������
    protected Evade _evade;

    // �и�
    private Parrying _parrying;
    #endregion

    #region �÷��̾� ������Ƽ
    // ���� ĳ���� ����
    public CharacterStatus charStats { get { return _status; } }

    // ���� ���� ����
    public PlayerBuffBuffer GetBuffBuffer { get { return _buffBuffer; } }

    // �и� ���� ����
    public bool ReturnCamParrying { get { return _parrying._returnCamParrying; } }

    // �и� �����ߴ��� ��ȭ����
    public bool GetOnParrying { get { return _parrying.OnParrying; } }

    // ���� ĳ���� ��ġ
    public Vector3 currentPos { get { return this.transform.position; } }

    // �÷��̾� �ִϸ�����
    public Animator GetAnimator { get { return _animator; } set { _animator = value; } }

    // �÷��̾� �ִ� ������Ʈ üũ
    public void SetInputAnyState(bool active) { _anyState = active; }

    // �÷��̾� isSpeed 0���� ����
    public void SetStop() { _isSpeed = 0f; moveMent = Vector3.zero; }

    // �÷��̾ InputMovement�ϴ��� üũ
    public bool IsMoving { get { return moveMent.sqrMagnitude > 0.01f; } }

    // �÷��̾ ��ų�� ���� �ִ��� �Ǻ�
    public bool GetSkillUsing { get { return _skillUsing; } set { _skillUsing = value; } }

    // �÷��̰� �⺻ ������ �ϰ� �ִ���
    public bool GetNormalAttackBool { get { return _normalAttackBool; } }

    // �÷��̾� ������
    public bool GetEvade { get { return _evade._evade; } }

    // �÷��̾� ����
    public bool GetHit { get; set; }

    // �÷��̾ �ൿ �� üũ
    public bool ReturnFirstCheck()
    {
        if (_anyState || _normalAttackBool || _skillUsing || !KeySet.GetMouseIsLock) return true;
        return false;
    }

    // ���¹̳� üũ
    public bool ReturnCheckStamina(float skillSta) => (GetCharHP.currentStamina >= skillSta);
    public CharacterHP GetCharHP { get { return _characterHP; } }
    #endregion

    #region ȣ�� ������Ƽ
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
        // ��ü ����
        _status = new CharacterStatus();
        _playerAnimState = new PlayerAnimState();
        _characterHP = new CharacterHP();
        _clickAttackDelay = new ClickAttackDelay();

        _playerUI = UIManager.Instance.GetPlayerUI;                 // Ui �Ŵ��� ����
    }
    private void CharInit()
    {
        // ������Ʈ ����
        rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _playerSkill = GetComponent<PlayerSkill>();
        _quickSlot = GetComponent<QuickSlot>();
        _evade = GetComponent<Evade>();
        _parrying = GetComponent<Parrying>();
        _buffBuffer = GetComponent<PlayerBuffBuffer>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        // �÷��̾� �ִϸ��̼� �ʱ�ȭ
        _playerAnimState.SetState(PlayerAnimState.AnimState.Idle);

        // �÷��̾� UI �ʱ�ȭ
        _playerUI.UICharacterInit();

        // ���� �ʱ�ȭ
        SetInputAnyState(false);

        // ���� �ʱ�ȭ
        CharStatusSetting();
    }


    protected void CharInputMoveKey()
    {
        if(!KeySet.GetMouseIsLock) { SetStop(); }

        moveMent.x = Input.GetAxisRaw("Horizontal");
        moveMent.z = Input.GetAxisRaw("Vertical");
        moveMent.y = 0;                                 // Y�� �������� �ʿ� �����Ƿ� �׻� 0���� ���� ���� X

        InputState();
    }

    private void KeyInput()
    {
        if(!AttackCheck() && _clickAttackDelay.CanAttackAfterMoveDelay())
        {
            if (Input.GetMouseButtonDown(0))
            {
                CharNormalAttack();                         // abstract �߻�ȭ�� �ʼ� ��� �ۼ� �䱸
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift)) SetInputRun(); // �޸��� Input

        void SetInputRun()                                      // �޸��� Anim Bool ����
        {
            _isRun = !_isRun;

            _playerUI.GetWRSprite = _playerUI.GetReturnWRSprite(_isRun);
        }

        bool AttackCheck()
        {
            return _skillUsing || _anyState || !KeySet.GetMouseIsLock || GetHit;
        }

    }

    private void InputState()       // �� ĳ������ �����̴� ���� �� ����
    {
        if (ReturnFirstCheck())
        {
            moveMent = Vector3.zero;
            return;
        }

        if (KeySet.MoveKeys.Any(Input.GetKey))                          // wasd�ƹ��ų� ������ �ִٸ�
        {
            if (!_isRun)
            { _playerAnimState.SetState(PlayerAnimState.AnimState.Walk); _isSpeed = _status._walkSpeed; }
            else { _playerAnimState.SetState(PlayerAnimState.AnimState.Run); _isSpeed = _status._runSpeed; }
        }
        else                                                               // �������� ������
        {
            _playerAnimState.SetState(PlayerAnimState.AnimState.Idle);
            _isSpeed = 0f;
        }
    }

    protected void CharMove()      // �� ĳ������ ������
    {
        // �̵� �ӵ� ����
        float moveSpeed = GetSpeed();

        // ī�޶� ���� �̵� ���� ���
        Vector3 camForward = cameraMove.GetCameraObject().transform.forward; // ī�޶��� ���� ����
        Vector3 camRight = cameraMove.GetCameraObject().transform.right;     // ī�޶��� ������ ����
        camForward.y = 0f; // Y�� ���� (���� �̵���)
        camRight.y = 0f;

        Vector3 moveDir = (camForward.normalized * moveMent.z + camRight.normalized * moveMent.x).normalized
                          * moveSpeed * Time.fixedDeltaTime;

        // �̵�
        rigidBody.MovePosition(rigidBody.position + moveDir);

        // ȸ��
        if (IsMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir); // �̵� ���� ���� ȸ��
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
    /// StatsNumber 0 => ü�� 1 => ���¹̳�
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
        if(!ReturnFirstCheck())                         // �ٸ� ���°� �ƴ��� üũ 
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
            if (col.gameObject.CompareTag("Enemy"))                         // ���� ���� �ٶ� �� 
            {
                Vector3 enemyPos = col.transform.position;
                transform.LookAt(enemyPos);
                break;
            }
        }
    }
}
