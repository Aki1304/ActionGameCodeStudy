using Unity.VisualScripting;
using UnityEngine;

public enum SkillClass { None = 0, AttackSkill, Buff, Passive, Active }
public enum BuffType { None, Attack, Critical, Def }

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/SkillData", order = int.MaxValue)]
public class SkillData : ScriptableObject
{
    [Header("��ų ���� ����")]
    [Space(10f)]
    [SerializeField, Tooltip("��ų ���� �̸�")] private string _skillName;
    [SerializeField, Tooltip("��ų �ΰ��� ǥ�� �̸�")] private string _InSkillName;
    [SerializeField, Tooltip("��ų ���¹̳� ��뷮")] private int _skillStamina;
    [SerializeField, Tooltip("��ų ��Ÿ��")] private float _coolDown;
    [SerializeField, Tooltip("��ų ���� ����")] private int _curLv;
    [SerializeField, Tooltip("��ų �ִ� ����")] private int _maxLv;
    [SerializeField, Tooltip("��ų ������")] private Sprite _skillSprite;
    [SerializeField, Tooltip("��ų ���� ĭ"), TextArea(3, 5)] private string _skillEx;

    [SerializeField, Tooltip("��ų ����")] private SkillClass _class;

    [Header("��ų ������ ���")]
    [Space(10f)]
    [SerializeField, Tooltip("���� ����")] private BuffType _buffType;
    [SerializeField, Tooltip("���� ���ӽð�")] private float _buffDuration;
    [SerializeField, Tooltip("���� ���")] private float _buffPercent;

    [Header("��ų ������ ���")]
    [Space(10f)]
    [SerializeField, Tooltip("��ų ��� / ������")] private float _AD;

    [Header("�нú��� ���")]
    [Space(10f)]
    [SerializeField, Tooltip("�нú� %")] private int _passivePercent;
    [SerializeField, Tooltip("Ÿ�� 1�� hp 2�� sp")] private int _passiveNum;


    private bool _skillRock = false;                                // ��ų�� ���ȴ��� �� ���ȴ��� �Ǻ�

    #region ������Ƽ
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
    /// �нú� ������ ����.
    /// 1 = maxHP 2 = masStamina
    /// </summary>
    public int SkillPassiveNum {  get { return _passiveNum; } }

    /// <summary>
    /// Skill �ѹ��� �����ϱ� ���� ����
    /// </summary>
    public int AttackSkillNum
    {
        get
        {
            foreach(var i in SkillName)
            {
                if (char.IsDigit(i)) return i - 48;             // 48�� �����ڵ� 0
            }

            return -1;      // ����ó��
        }
    }
    /// <summary>
    /// True ��� False ����
    /// </summary>
    public bool SkillRock { get { return _skillRock; } set { _skillRock = value; } }
    public int SkillMaxLV { get { return _maxLv; } }
    public int SkillCurLv { get { return _curLv; } set { _curLv = value; } }

    #endregion

    #region ��ų����Ʈ ��� ����
    /// <summary>
    /// ��ų ����Ʈ ReturnPossible�� ���� �Ұ� �˻�
    /// True �� ��ų ����Ʈ ��밡���Ѱ����� �ٽ� maxLev���� �˻�
    /// </summary>
    /// <param name="active"></param>
    public PointUseUI SkillLevelUP(bool active)
    {
        if (active)
        {
            if (CanLevelUp()) 
            { 
                _curLv++;                       // ���� ���� ��
                SkillRock = IsSkillRock();
                return PointUseUI.Possible; 
            }
            else { return PointUseUI.MaxLev; }
        }
        else return PointUseUI.Impossible;
    }

    /// <summary>
    /// �������� �������� �Ǻ�
    /// </summary>
    /// <returns></returns>
    public bool CanLevelUp() => _curLv < _maxLv;
    #endregion

    #region ��ų �� ����
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
