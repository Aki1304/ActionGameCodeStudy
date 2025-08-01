using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameTabUI : MonoBehaviour
{
    #region ��ü ����
    [Header("TabUI ������Ʈ")]
    [SerializeField] private GameObject _TabObject;
    
    private RectTransform _tabRect;

    [Header("TabAni")]
    [SerializeField] private RightBarAnim _barAnim;

    [Space(10)]
    [Header("��ų ��")]
    [SerializeField] private GameObject _skillTab;
    [SerializeField, Tooltip("��ų ���� ��ũ��Ʈ")] private SkillTabInfo _skillTabInfo;

    [Space(10)]
    [Header("���� ��")]
    [SerializeField] private GameObject _statTab;
    [SerializeField, Tooltip("���� ���� ��ũ��Ʈ")] private StatusTanInfo _statsTabInfo;
    
    [Space(10)]
    [Header("�� �� �ڽ�")]
    [SerializeField, Tooltip("�� ������ UI ���")] public WindowUI _windowUI;
    #endregion

    #region ������Ƽ
    public UIManager UM { get { return UIManager.Instance; } }
    public KeySetting KeySet { get { return GameManager.Instance.KeySet; } set { GameManager.Instance.KeySet = value; } }
    public SkillTabInfo GetSkillTab { get { return _skillTabInfo; } }
    public StatusTanInfo GetStatTab { get { return _statsTabInfo; } }
    public bool GetMouseLock { get { return KeySet.GetMouseIsLock; } set { KeySet.GetMouseIsLock = value; } }
    #endregion

    #region UI �ʱ�ȭ ����
    public void Clear()
    {
        _skillTab.SetActive(false);
        _statTab.SetActive(false);
        _skillTabInfo.TabClear();
    }
    #endregion

    private void Awake()
    {
        // ��ü ����
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
