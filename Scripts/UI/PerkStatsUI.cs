using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkStatsUI : MonoBehaviour
{
    [SerializeField] private Image _perkImage;
    [SerializeField] private Text _perkName;
    [SerializeField] private Text _perkLevel;
    [SerializeField] private Text _perkEx;

    public Perk ReturnPerk { get; set; }

    public void SetPerkUI()
    {
        _perkImage.sprite = ReturnPerk.PerkSprite;
        _perkName.text = ReturnPerk.PerkTooltip;
        _perkLevel.text = ReturnPerk.PerkEffect.StringToLevel();
        _perkEx.text = ReturnPerk.PerkEffect.StringToMainToolTip();
    }
}
