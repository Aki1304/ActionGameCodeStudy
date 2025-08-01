using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleTon<UIManager>
{
    #region UI ������Ʈ ��ũ��Ʈ
    /// <summary>
    /// 1. �÷��̾� ui 2. �÷��̾� ����â 3. Option ui 
    /// 4. ���� ui 5. ����Ʈ ���� 
    /// </summary>


    [Header("�÷��̾� UI")]
    [SerializeField, Tooltip("�÷��̾� UI ��ũ��Ʈ")] private PlayerUI _playerUI;

    [Header("Game ETC UI")]
    [SerializeField, Tooltip("���� UI ��ũ��Ʈ")] private GameTabUI _gameTabUI;

    [Header("������ Ǯ��")]
    [SerializeField, Tooltip("������ Ǯ�� ��ũ��Ʈ")] private DamagePooling _dmgPooling;

    [Header("���� UI")]
    [SerializeField, Tooltip("���� UI ��ũ��Ʈ")] private DungeonUI _dungeonUI;
    #endregion

    #region ȣ��
    public PlayerUI GetPlayerUI { get { return Instance._playerUI; } set { Instance._playerUI = value; } }
    public GameTabUI GetGameUI { get { return Instance._gameTabUI; } set { Instance._gameTabUI = value; } }
    public DamagePooling GetDMGPoolUI { get { return Instance._dmgPooling; } }
    public DungeonUI GetDGUI { get { return Instance._dungeonUI; } }
    public WindowUI GetWindowUI { get { return GetGameUI._windowUI; } set { GetGameUI._windowUI = value; } }

    #endregion

    private void Awake()
    {
        UIAllClear();
    }


    #region UI Init �ʱ�ȭ �س���
    private void UIAllClear()
    {
        GetGameUI.Clear();
        GetDGUI.InitClass();
    }
    #endregion

    #region Tab ������ ���� �ִ� ���õ� ��� �ʱ�ȭ
    public void OepnCloseUI()
    {
        GetGameUI.Clear();
        if(GetWindowUI.gameObject.activeSelf) GetWindowUI.DisableWindowUI();
    }
    #endregion

}
