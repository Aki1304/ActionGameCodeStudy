using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;


public class SkillTabInfo : MonoBehaviour
{
    #region ������Ʈ
    [Header("��ų ������")]
    [SerializeField] private Transform _iconSkillTrans;

    [Header("��ų ���� ��")]
    [SerializeField] private GameObject _infoTab;

    [Header("��ų ���� ĭ")]
    [SerializeField, Tooltip("��ų ���� ĭ �̹���")] private Image _selectImage;
    [SerializeField, Tooltip("��ų �̸�")] private Text _selectName;
    [SerializeField, Tooltip("��ų ����")] private Text _selectLevel;
    [SerializeField, Tooltip("��ų ����Ʈ")] private Text _skillPoint;
    [SerializeField, Tooltip("��ų Ŭ����")] private Text _selectClass;
    [SerializeField, Tooltip("��ų ��� ��")] private GameObject _selectSlot;
    [SerializeField, Tooltip("��ų ���� ĭ")] private Text _selectInfo;

    [Header("���� ���� ��������Ʈ")]
    [SerializeField] private Sprite[] _selectSprites;

    private List<Image> _iconSkills = new List<Image>();                // ���� ȭ�� ǥ�� ������ ������Ʈ
    #endregion

    #region ������Ƽ
    private PlayerManager PM { get { return PlayerManager.Instance; } }
    private PlayerSkill _playerSkill { get { return PM.GetPlayer.GetPlayerSkill; } }
    private List<SkillData> _skillDatas { get { return PM.GetSkillData; } }        // ��ų ������
    public SkillData _selectSkillData { get; set; }                                 // ������ ��ų������
    public WindowUI _windowUI { get { return UIManager.Instance.GetWindowUI; } }
    public QuickSlotUI GetQuickUI { get { return _quickSlotUI; } } 
    private bool ReturnPossibleSkill() => _playerSkill.ReturnPossibleSkillRankUP();
    #endregion

    #region ��ü ����
    private StringUI SetStringUI = new StringUI();
    private QuickSlotUI _quickSlotUI;
    #endregion

    #region ��ư ����
    public void SelectIconButton(int number)                            // ������ ��ų ������ ��
    {
        // ���� ��ų ������ ���� �Ѱ� ����
        for(int i = 0; i < _iconSkills.Count; i++)
        {
            if (i == number) continue;
            _iconSkills[i].sprite = _selectSprites[0];
        }

        _iconSkills[number].sprite = _selectSprites[1];

        if (!_infoTab.activeSelf) _infoTab.SetActive(true);
        SetSkilTabInfo(_skillDatas[number]);
    }

    public void SkillPointUse()                                         // ��ų ����Ʈ ����ϱ� ������ ���
    {
        bool possible = ReturnPossibleSkill();                          // ��ų ������ ���� ���� �Ǻ� 
        PointUseUI result = _selectSkillData.SkillLevelUP(possible);    // ��ų ui �����

        _windowUI.EnableWindowUI(SetStringUI.ReturnWindowString(result, _selectSkillData));                              // UI ǥ��
        SkillEffect();
        SetSkilTabInfo(_selectSkillData);

        void SkillEffect()
        {
            if (result == PointUseUI.Possible)              // ����� �ȴٸ�
            {
                _playerSkill.DownSkillPoint();
            }

        }
    }

    public void SelectQuickSlot()
    {
        _quickSlotUI.UpdateButton();
    }
    #endregion

    private void Start()
    {
        ComponentInit();
    }

    private void ComponentInit()
    {
        // ������Ʈ ����
        _quickSlotUI = GetComponent<QuickSlotUI>();

        _infoTab.SetActive(false);   // ���� â ������.

        // ����Ʈ�� ��ų ���
        int iconNum = _iconSkillTrans.childCount;

        for(int i = 0; i < iconNum; i++)
        {
            Image iconimg = _iconSkillTrans.GetChild(i).GetComponent<Image>();
            _iconSkills.Add(iconimg);

        }
    }

    public void TabClear()
    {
        _infoTab.SetActive(false);
        if (_quickSlotUI != null) _quickSlotUI.ActiveQuickWindow(false);
        foreach (Image img in _iconSkills) img.sprite = _selectSprites[0];
        SetSkilTabInfo(null);
    }

    public void SetSkilTabInfo(SkillData skill)
    {
        if(skill == null)                // �� �ʱ�ȭ ��
        {
            _selectSkillData = null;
            _selectImage.sprite = null;
            _selectName.text = _selectLevel.text = _selectClass.text = _selectInfo.text = null;
        }
        else          // ��ų ���� ��
        {
            _selectSkillData = skill;
            _selectImage.sprite = skill.SkillSprite;
            _selectName.text = skill.InSkillName;
            _selectLevel.text = string.Format("[ {0} / {1} ]", skill.SkillCurLv, skill.SkillMaxLV);
            _skillPoint.text = string.Format("SP : {0}", PM.GetPlayerSkillPoint);
            _selectClass.text = string.Format("[ {0} ]", SetStringUI.ReturnKor(skill.SkillClass));
            _selectInfo.text = SetStringUI.ReturnStringEx(_selectSkillData);

            OnSlotManager();
        }


    }

    public void OnSlotManager()
    {
        SkillClass selectClass = _selectSkillData.SkillClass;

        bool Active =
        (selectClass == SkillClass.AttackSkill || selectClass == SkillClass.Buff) ? true : false;

        _selectSlot.SetActive(Active);
    }

}
