using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverPerk : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Outline _outline;

    public Perk ReturnPerk { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        _outline.enabled = false;

        ReturnPerk.Apply();
        UIManager.Instance.GetDGUI._perUI.EndPick();
        UIManager.Instance.GetGameUI.GetStatTab.PerkUpdateUI(ReturnPerk);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }

    public void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }


}
