using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour
{
    [Header("��ų ����")]
    [SerializeField] private Transform _slotIconTrans;

    [Header("QuickSlotUI GameObject")]
    [SerializeField] private GameObject _quickWindow;

    [Header("QuickSlotUI Text")]
    [SerializeField] private Text _text;

    private List<Image> _slotRender = new List<Image>();                // ���� ������ ������Ʈ
    private StringUI _stringUI = new StringUI();
    private SkillTabInfo _skillTabInfo;

    private bool _fail = false;
    private bool _success = false;
    private int _indexNum;
    #region ������Ƽ
    private PlayerManager PM { get { return PlayerManager.Instance; } }
    private UIManager UM { get { return UIManager.Instance; } }
    private KeySetting GetKeySet { get { return GameManager.Instance.KeySet; } }
    private SkillData SelectSkillData { get { return _skillTabInfo._selectSkillData; } }
    #endregion

    #region ��� �Լ�
    public void ActiveQuickWindow(bool active) => _quickWindow.SetActive(active);
    #endregion

    private void Start()
    {
        ComponentInit();
    }

    private void Update()
    {
        KeyInput();
    }

    private void KeyInput()
    {
        if (_fail)
        {
            if (Input.anyKeyDown) ActiveQuickWindow(false);
        }

        if(_success)
        {
            if (GetKeySet.QuickKeys.Any(Input.GetKeyDown))
            {
                _indexNum = Array.FindIndex(GetKeySet.QuickKeys, Input.GetKeyDown);
                _success = false;
            }
        }
    }

    private void ComponentInit()
    {
        // ��ü ����
        _skillTabInfo = GetComponent<SkillTabInfo>();

        // ����Ʈ ����
        int slotNum = _slotIconTrans.childCount;

        for (int i = 0; i < slotNum; i++)
        {
            Image slotimg = _slotIconTrans.GetChild(i).GetChild(0).GetComponent<Image>();
            _slotRender.Add(slotimg);
        }
    }

    public void UpdateButton()
    {
        bool possible = !_skillTabInfo._selectSkillData.IsSkillRock();                // possible�� Rock�� True�� false

        _text.text = _stringUI.ReturnQuickSlotString(possible);
        if (!_quickWindow.activeSelf) StartCoroutine(QuickSlotChange());

        IEnumerator QuickSlotChange()
        {
            _quickWindow.SetActive(true);           // �ѱ�

            if (!possible) { _fail = true; yield break; }       // ���� ���� ��� ����������
            else _success = true;

            yield return new WaitUntil(() => CheckQuickUI());
            ActiveQuickWindow(false);                           // ����
        }

        bool CheckQuickUI()
        {
            if (!_success)
            {
                PM.UpdateQuickSlot(SelectSkillData, _indexNum);      // ���� ��ų�� �ε��� ��ȣ �ѱ��
                return true; 
            }

            if (!UM.GetGameUI.GetSkillTab.gameObject.activeSelf) return true;
            else return false;
        }

    }


    public void SpriteChange(int checkNum = 0, bool active = true)
    {
        Sprite _sp = (active) ? SelectSkillData.SkillSprite : null;
        int choiceNum = (active) ? _indexNum : checkNum;
        int alpha = (active) ? 1 : 0;

        // Main QuickUI
        UM.GetPlayerUI.PassMainQuickSprite(_sp, choiceNum, active);

        // Tab QuickUI
        _slotRender[choiceNum].sprite = _sp;
        _slotRender[choiceNum].color = new Color(1, 1, 1, alpha);
    }
}
