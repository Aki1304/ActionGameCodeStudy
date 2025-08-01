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
    #region 컴포넌트
    [Header("스킬 아이콘")]
    [SerializeField] private Transform _iconSkillTrans;

    [Header("스킬 인포 탭")]
    [SerializeField] private GameObject _infoTab;

    [Header("스킬 설명 칸")]
    [SerializeField, Tooltip("스킬 설명 칸 이미지")] private Image _selectImage;
    [SerializeField, Tooltip("스킬 이름")] private Text _selectName;
    [SerializeField, Tooltip("스킬 레벨")] private Text _selectLevel;
    [SerializeField, Tooltip("스킬 포인트")] private Text _skillPoint;
    [SerializeField, Tooltip("스킬 클래스")] private Text _selectClass;
    [SerializeField, Tooltip("스킬 등록 탭")] private GameObject _selectSlot;
    [SerializeField, Tooltip("스킬 설명 칸")] private Text _selectInfo;

    [Header("선택 비선택 스프라이트")]
    [SerializeField] private Sprite[] _selectSprites;

    private List<Image> _iconSkills = new List<Image>();                // 왼쪽 화면 표시 아이콘 컴포넌트
    #endregion

    #region 프로퍼티
    private PlayerManager PM { get { return PlayerManager.Instance; } }
    private PlayerSkill _playerSkill { get { return PM.GetPlayer.GetPlayerSkill; } }
    private List<SkillData> _skillDatas { get { return PM.GetSkillData; } }        // 스킬 데이터
    public SkillData _selectSkillData { get; set; }                                 // 선택한 스킬데이터
    public WindowUI _windowUI { get { return UIManager.Instance.GetWindowUI; } }
    public QuickSlotUI GetQuickUI { get { return _quickSlotUI; } } 
    private bool ReturnPossibleSkill() => _playerSkill.ReturnPossibleSkillRankUP();
    #endregion

    #region 객체 생성
    private StringUI SetStringUI = new StringUI();
    private QuickSlotUI _quickSlotUI;
    #endregion

    #region 버튼 관련
    public void SelectIconButton(int number)                            // 아이콘 스킬 눌렀을 때
    {
        // 누른 스킬 아이콘 제외 켜고 끄기
        for(int i = 0; i < _iconSkills.Count; i++)
        {
            if (i == number) continue;
            _iconSkills[i].sprite = _selectSprites[0];
        }

        _iconSkills[number].sprite = _selectSprites[1];

        if (!_infoTab.activeSelf) _infoTab.SetActive(true);
        SetSkilTabInfo(_skillDatas[number]);
    }

    public void SkillPointUse()                                         // 스킬 포인트 사용하기 눌렀을 경우
    {
        bool possible = ReturnPossibleSkill();                          // 스킬 레벨업 가능 여부 판별 
        PointUseUI result = _selectSkillData.SkillLevelUP(possible);    // 스킬 ui 결과값

        _windowUI.EnableWindowUI(SetStringUI.ReturnWindowString(result, _selectSkillData));                              // UI 표시
        SkillEffect();
        SetSkilTabInfo(_selectSkillData);

        void SkillEffect()
        {
            if (result == PointUseUI.Possible)              // 사용이 된다면
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
        // 컴포넌트 기초
        _quickSlotUI = GetComponent<QuickSlotUI>();

        _infoTab.SetActive(false);   // 선택 창 꺼놓기.

        // 리스트에 스킬 담기
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
        if(skill == null)                // 탭 초기화 시
        {
            _selectSkillData = null;
            _selectImage.sprite = null;
            _selectName.text = _selectLevel.text = _selectClass.text = _selectInfo.text = null;
        }
        else          // 스킬 선택 시
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
