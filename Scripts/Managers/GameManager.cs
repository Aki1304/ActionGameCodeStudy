using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    private KeySetting _keySetting;        // 키 세팅
    private PerkManager _perkManager;

    #region 프로퍼티
    public KeySetting KeySet { get { return _keySetting; } set { _keySetting = value; } }
    public PerkManager GetPM { get { return _perkManager; } }
    #endregion
    private void Awake()
    {
        InitClass();
    }

    private void InitClass()
    {
        _keySetting = GetComponent<KeySetting>();
        _perkManager = GetComponent<PerkManager>();
    }
}
