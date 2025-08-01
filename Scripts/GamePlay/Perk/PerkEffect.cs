using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerkEffect : ScriptableObject
{
    /// <summary>
    /// ���� ���� �̰ɷ� �� ���� �� �͵��� ����..
    /// </summary>
    /// 
    public abstract void ApplyEffect();

    /// <summary>
    /// ���� �� ��Ʈ������ ����
    /// </summary>
    /// <returns></returns>
    public abstract string StringToLevel();
    public abstract string StringToMainToolTip();
    public abstract string StringToPickToolTip();
    public abstract void InitPerk();
}
