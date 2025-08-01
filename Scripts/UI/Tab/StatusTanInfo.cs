using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusTanInfo : MonoBehaviour
{
    [SerializeField] private Text StatsText;
    [SerializeField] private Transform _perkMainTransform;
    [SerializeField] private GameObject _prefabPerk;
    public Transform GetPerkMainTransform { get { return _perkMainTransform; } }

    private List<GameObject> _mainToolTipObjects = new List<GameObject>();


    #region 프로퍼티
    public PlayerManager PM { get  { return PlayerManager.Instance; } }
    public Player GetPlayer { get { return PM.GetPlayer; } }
    public CharacterStatus GetStats { get { return GetPlayer.charStats; } }
    #endregion


    public void TabOpen()
    {
        bool active = this.gameObject.activeSelf;

        if(active)
        {
            UpdateStats();
        }

    }

    public void UpdateStats(int percent = 0)
    {
        StatsUpdateUI();
    }


    void StatsUpdateUI()
    {
        StatsText.text = 
            string.Format("캐릭터 : {0}\n최대 체력 : {1}\n최대 스태미나 : {2}\n\n공격력 : {3}\n방어력 : {4}\n크리티컬 : {5}"
            , GetStats._charName, GetStats._maxHP,GetStats._maxStamina,GetStats.Attack,GetStats._defDmg,GetStats.Critical);
    }
    
    public void PerkUpdateUI(Perk perk)
    {
        GameObject pickObject;

        bool check = false;

        foreach(var obj in _mainToolTipObjects)
        {
            PerkStatsUI statsUI = obj.GetComponent<PerkStatsUI>();
            if (statsUI.ReturnPerk == perk) { check = true; statsUI.SetPerkUI(); break; }
        }    

        if(!check)
        {
            pickObject = Instantiate(_prefabPerk,_perkMainTransform);

            PerkStatsUI statsUI = pickObject.GetComponent<PerkStatsUI>();

            statsUI.ReturnPerk = perk;
            statsUI.SetPerkUI();

            _mainToolTipObjects.Add(pickObject);
        }
    }
}
