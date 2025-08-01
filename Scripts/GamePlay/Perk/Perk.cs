using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perk",menuName = "Scriptable Object/Perks/Perk")]
public class Perk : ScriptableObject
{
    [SerializeField] private string _perkName;          // 퍽 이름
    [SerializeField] private string _toolTipName;       // 툴팁 표기 할 이름
    [SerializeField] private bool _perkLock;            // 퍽 얻었는지 안 얻었는지 

    [SerializeField] private Sprite _perkSprite;        // 퍽 이미지 필요할지 안할지는 모르겠음
    [SerializeField] private PerkEffect _perkEffect;    // 실제 퍽 적용


    public string PerkName { get { return _perkName; } }
    public string PerkTooltip { get { return _toolTipName; } }
    public bool PerkLock {  get { return _perkLock; } }
    public Sprite PerkSprite { get { return _perkSprite; } }
    public PerkEffect PerkEffect { get { return _perkEffect; } }

    public void Apply() => _perkEffect?.ApplyEffect();

}
