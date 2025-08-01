using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perk",menuName = "Scriptable Object/Perks/Perk")]
public class Perk : ScriptableObject
{
    [SerializeField] private string _perkName;          // �� �̸�
    [SerializeField] private string _toolTipName;       // ���� ǥ�� �� �̸�
    [SerializeField] private bool _perkLock;            // �� ������� �� ������� 

    [SerializeField] private Sprite _perkSprite;        // �� �̹��� �ʿ����� �������� �𸣰���
    [SerializeField] private PerkEffect _perkEffect;    // ���� �� ����


    public string PerkName { get { return _perkName; } }
    public string PerkTooltip { get { return _toolTipName; } }
    public bool PerkLock {  get { return _perkLock; } }
    public Sprite PerkSprite { get { return _perkSprite; } }
    public PerkEffect PerkEffect { get { return _perkEffect; } }

    public void Apply() => _perkEffect?.ApplyEffect();

}
