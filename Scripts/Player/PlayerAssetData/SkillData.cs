using Unity.VisualScripting;
using UnityEngine;

public enum SkillClass { None = 0, AttackSkill, Buff, Passive, Active }
public enum BuffType { None, Attack, Critical, Def }

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/SkillData", order = int.MaxValue)]
public class SkillData : ScriptableObject
{
    [Header("스킬 기초 설정")]
    [Space(10f)]
    [SerializeField, Tooltip("스킬 구분 이름")] private string _skillName;
    [SerializeField, Tooltip("스킬 인게임 표기 이름")] private string _InSkillName;
    [SerializeField, Tooltip("스킬 스태미나 사용량")] private int _skillStamina;
    [SerializeField, Tooltip("스킬 쿨타임")] private float _coolDown;
    [SerializeField, Tooltip("스킬 현재 레벨")] private int _curLv;
    [SerializeField, Tooltip("스킬 최대 레벨")] private int _maxLv;
    [SerializeField, Tooltip("스킬 아이콘")] private Sprite _skillSprite;
    [SerializeField, Tooltip("스킬 설명 칸"), TextArea(3, 5)] private string _skillEx;

    [SerializeField, Tooltip("스킬 종류")] private SkillClass _class;

    [Header("스킬 버프일 경우")]
    [Space(10f)]
    [SerializeField, Tooltip("버프 종류")] private BuffType _buffType;
    [SerializeField, Tooltip("버프 지속시간")] private float _buffDuration;
    [SerializeField, Tooltip("버프 계수")] private float _buffPercent;

    [Header("스킬 공격일 경우")]
    [Space(10f)]
    [SerializeField, Tooltip("스킬 계수 / 데미지")] private float _AD;

    [Header("패시브일 경우")]
    [Space(10f)]
    [SerializeField, Tooltip("패시브 %")] private int _passivePercent;
    [SerializeField, Tooltip("타입 1은 hp 2는 sp")] private int _passiveNum;


    private bool _skillRock = false;                                // 스킬이 열렸는지 안 열렸는지 판별

    #region 프로퍼티
    public string SkillName { get { return _skillName; } }
    public string InSkillName { get { return _InSkillName; } }
    public SkillClass SkillClass { get { return _class; } }
    public BuffType BuffType { get { return _buffType; } }
    public float SkillCoolDown { get { return _coolDown; } }
    public int SkillStaminaUse { get { return _skillStamina; } }
    public float SkillAttackDamage { get { return _AD; } }
    public string SkillEx { get { return _skillEx; } }
    public float SkillBuffDuration { get { return _buffDuration; } }
    public float SkillBuffPercent { get { return _buffPercent; } }
    public Sprite SkillSprite { get { return _skillSprite; } }
    public int SkillPassivePercent { get { return _passivePercent; } }

    /// <summary>
    /// 패시브 구분을 위함.
    /// 1 = maxHP 2 = masStamina
    /// </summary>
    public int SkillPassiveNum {  get { return _passiveNum; } }

    /// <summary>
    /// Skill 넘버를 구분하기 위한 숫자
    /// </summary>
    public int AttackSkillNum
    {
        get
        {
            foreach(var i in SkillName)
            {
                if (char.IsDigit(i)) return i - 48;             // 48이 유니코드 0
            }

            return -1;      // 예외처리
        }
    }
    /// <summary>
    /// True 잠김 False 열림
    /// </summary>
    public bool SkillRock { get { return _skillRock; } set { _skillRock = value; } }
    public int SkillMaxLV { get { return _maxLv; } }
    public int SkillCurLv { get { return _curLv; } set { _curLv = value; } }

    #endregion

    #region 스킬포인트 사용 여부
    /// <summary>
    /// 스킬 포인트 ReturnPossible로 먼저 불값 검사
    /// True 면 스킬 포인트 사용가능한거지만 다시 maxLev으로 검사
    /// </summary>
    /// <param name="active"></param>
    public PointUseUI SkillLevelUP(bool active)
    {
        if (active)
        {
            if (CanLevelUp()) 
            { 
                _curLv++;                       // 현재 레벨 업
                SkillRock = IsSkillRock();
                return PointUseUI.Possible; 
            }
            else { return PointUseUI.MaxLev; }
        }
        else return PointUseUI.Impossible;
    }

    /// <summary>
    /// 레벨업이 가능한지 판별
    /// </summary>
    /// <returns></returns>
    public bool CanLevelUp() => _curLv < _maxLv;
    #endregion

    #region 스킬 락 여부
    public bool IsSkillRock()
    {
        return (_curLv == 0);
    }
    #endregion
   

    public float PercentFormula()
    {
        float percent =
        (this.SkillClass == SkillClass.Buff) ? this.SkillBuffPercent :
        (this.SkillClass == SkillClass.Passive) ? this.SkillPassivePercent : 0;

        return (this.SkillClass == SkillClass.Passive) ? percent : percent * SkillCurLv;
    }

}
