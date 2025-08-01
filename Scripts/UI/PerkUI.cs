using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _pickPerks;           // ���� ������Ʈ
    public PerkManager PerkM { get { return GameManager.Instance.GetPM; } }


    public void PerkInit()
    {
        foreach (var p in _pickPerks) { p.SetActive(false); }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha2))
        { PickPerks(); }    
    }

    public void EndPick()
    {
        foreach(var p in _pickPerks) { p.gameObject.SetActive(false); }
        // ���� ���콺 �� �ɱ�
        GameManager.Instance.KeySet.InputMouseLock();
    }

    public void PickPerks()
    {
        // ���� ���콺 �� ���� Ǯ���ֱ�
        GameManager.Instance.KeySet.InputMouseLock();

        int[] tmpCheckArray = new int[3];               // �ӽ� �迭 ���� �ؼ� ���� ���� ����
        int count = 0;                                  // �迭 ī��Ʈ ��Ű�� �뵵
        for(int i = 0; i < 3; i++)
        {
            int randomPick = RanomChecking(i);
            tmpCheckArray[i] = randomPick;
        }
       

        // ���� ����� �ܵ� ���� ��Ű��
        foreach (var p in _pickPerks) 
        {
            int number = tmpCheckArray[count];                                   // �� �ε��� ���� ��ȣ ��.

            Image image = p.transform.GetChild(1).GetComponent<Image>();         // �̹���
            Text name = p.transform.GetChild(2).GetComponent<Text>();            // �� �̸�
            Text level = p.transform.GetChild(3).GetComponent<Text>();           // �� ����
            Text ex = p.transform.GetChild(4).GetComponent<Text>();              // �� ����

            image.sprite = PerkM.GetGamePerks[number].PerkSprite;
            name.text = PerkM.GetGamePerks[number].PerkTooltip;
            level.text = PerkM.GetGamePerks[number].PerkEffect.StringToLevel();
            ex.text =  PerkM.GetGamePerks[number].PerkEffect.StringToPickToolTip();

            // ������� �ܿ� ���� �־��� �� ����
            HoverPerk hoverPerk = _pickPerks[count].GetComponent<HoverPerk>();
            hoverPerk.ReturnPerk = PerkM.GetGamePerks[number];                  // �� �־��ֱ�

            _pickPerks[count].gameObject.SetActive(true);                       // �� UI Ű��
            count++;
        }



        int RanomChecking(int idx)
        {
            int randNum;
            while (true)
            {
                randNum = Random.Range(0, PerkM.GetGamePerks.Length);            // ���� �� ���̱��� �������� ���� �̱�
                bool isDupleCheck = false;


                // ù ���ڴ� �ֱ�
                if (idx == 0) break;

                // ù ���� ���� �ݺ� �˻�
                for(int i = 0; i < tmpCheckArray.Length; i++)
                {
                    if (tmpCheckArray[i] == randNum)
                    {
                        isDupleCheck = true;
                        break;
                    }
                }

                // �ݺ��� ���ǿ� �ɸ��� �ʾҴٸ� return ���ֱ�
                if (!isDupleCheck) break;
            }

            return randNum;
        }
    }

}
