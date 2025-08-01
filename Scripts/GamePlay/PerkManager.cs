using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PerkManager : MonoBehaviour
{
    private float _perkNormalAttackSpeed = 0f;

    public float GetNormalAttackPerk { get { return _perkNormalAttackSpeed; } set { _perkNormalAttackSpeed = value; } }

    [SerializeField] private Perk[] _inGamePerks = new Perk[6];

    public Perk[]  GetGamePerks { get { return _inGamePerks; } }

    // �ӽ� �������� ����� �ϴ� ����
    public List<SlashWork> GetSlash = new List<SlashWork>();


    private void Start()
    {
        foreach (var p in _inGamePerks) p.PerkEffect.InitPerk();
    }
}
