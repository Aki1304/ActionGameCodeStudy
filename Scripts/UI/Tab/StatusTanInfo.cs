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


    #region ������Ƽ
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
            string.Format("ĳ���� : {0}\n�ִ� ü�� : {1}\n�ִ� ���¹̳� : {2}\n\n���ݷ� : {3}\n���� : {4}\nũ��Ƽ�� : {5}"
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
