using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameTabUI : MonoBehaviour
{
    #region 객체 생성
    [Header("TabUI 오브젝트")]
    [SerializeField] private GameObject _TabObject;
    
    private RectTransform _tabRect;

    [Header("TabAni")]
    [SerializeField] private RightBarAnim _barAnim;

    [Space(10)]
    [Header("스킬 탭")]
    [SerializeField] private GameObject _skillTab;
    [SerializeField, Tooltip("스킬 인포 스크립트")] private SkillTabInfo _skillTabInfo;

    [Space(10)]
    [Header("스탯 탭")]
    [SerializeField] private GameObject _statTab;
    [SerializeField, Tooltip("스탯 인포 스크립트")] private StatusTanInfo _statsTabInfo;
    
    [Space(10)]
    [Header("빈 글 박스")]
    [SerializeField, Tooltip("빈 윈도우 UI 사용")] public WindowUI _windowUI;
    #endregion

    #region 프로퍼티
    public UIManager UM { get { return UIManager.Instance; } }
    public KeySetting KeySet { get { return GameManager.Instance.KeySet; } set { GameManager.Instance.KeySet = value; } }
    public SkillTabInfo GetSkillTab { get { return _skillTabInfo; } }
    public StatusTanInfo GetStatTab { get { return _statsTabInfo; } }
    public bool GetMouseLock { get { return KeySet.GetMouseIsLock; } set { KeySet.GetMouseIsLock = value; } }
    #endregion

    #region UI 초기화 전용
    public void Clear()
    {
        _skillTab.SetActive(false);
        _statTab.SetActive(false);
        _skillTabInfo.TabClear();
    }
    #endregion

    private void Awake()
    {
        // 객체 생성
        _tabRect = _TabObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UseTab();
    }

    private void Init()
    {
        _barAnim.SetPlaying(false);
    }

    private void UseTab()
    {
        if(KeySet.InputKey(KeySet._tab))
        {
            if (_barAnim.GetCheckAnim())
            {
                _barAnim.SetPlaying(true);
                StartCoroutine(_barAnim.MoveBar(_tabRect));
                KeySet.InputMouseLock();
            }
        }
    }

    public void LinkTab(int number)
    {
        GameObject curObject = ReturnObject();

        curObject.SetActive(!curObject.activeSelf);
        CheckActive();

        GameObject ReturnObject()
        {
            return number switch
            {
                0 => _skillTab,
                1 => _statTab,
                _ => null
            };
        }

        void CheckActive()
        {
            switch (number)
            {
                case 0:
                    _statTab.SetActive(false);
                    _skillTabInfo.TabClear();
                    break;
                case 1:
                    _skillTab.SetActive(false);
                    _statsTabInfo.TabOpen();
                    break;
                default: break;
            }
        }
    }
}
