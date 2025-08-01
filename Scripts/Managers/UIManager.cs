using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleTon<UIManager>
{
    #region UI 컴포넌트 스크립트
    /// <summary>
    /// 1. 플레이어 ui 2. 플레이어 스탯창 3. Option ui 
    /// 4. 몬스터 ui 5. 이펙트 관리 
    /// </summary>


    [Header("플레이어 UI")]
    [SerializeField, Tooltip("플레이어 UI 스크립트")] private PlayerUI _playerUI;

    [Header("Game ETC UI")]
    [SerializeField, Tooltip("게임 UI 스크립트")] private GameTabUI _gameTabUI;

    [Header("데미지 풀링")]
    [SerializeField, Tooltip("데미지 풀링 스크립트")] private DamagePooling _dmgPooling;

    [Header("던전 UI")]
    [SerializeField, Tooltip("던전 UI 스크립트")] private DungeonUI _dungeonUI;
    #endregion

    #region 호출
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


    #region UI Init 초기화 해놓기
    private void UIAllClear()
    {
        GetGameUI.Clear();
        GetDGUI.InitClass();
    }
    #endregion

    #region Tab 누를시 켜져 있는 세팅들 모두 초기화
    public void OepnCloseUI()
    {
        GetGameUI.Clear();
        if(GetWindowUI.gameObject.activeSelf) GetWindowUI.DisableWindowUI();
    }
    #endregion

}
