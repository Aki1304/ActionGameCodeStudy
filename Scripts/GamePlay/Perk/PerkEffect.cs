using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerkEffect : ScriptableObject
{
    /// <summary>
    /// 참조 받은 이걸로 실 적용 할 것들을 만듦..
    /// </summary>
    /// 
    public abstract void ApplyEffect();

    /// <summary>
    /// 레벨 값 스트링으로 전달
    /// </summary>
    /// <returns></returns>
    public abstract string StringToLevel();
    public abstract string StringToMainToolTip();
    public abstract string StringToPickToolTip();
    public abstract void InitPerk();
}
