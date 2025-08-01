using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    private KeySetting _keySetting;        // Ű ����
    private PerkManager _perkManager;

    #region ������Ƽ
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
