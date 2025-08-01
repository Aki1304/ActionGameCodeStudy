using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _pickPerks;           // 상위 오브젝트
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
        // 먼저 마우스 락 걸기
        GameManager.Instance.KeySet.InputMouseLock();
    }

    public void PickPerks()
    {
        // 먼저 마우스 락 부터 풀어주기
        GameManager.Instance.KeySet.InputMouseLock();

        int[] tmpCheckArray = new int[3];               // 임시 배열 생성 해서 랜덤 난수 생성
        int count = 0;                                  // 배열 카운트 시키는 용도
        for(int i = 0; i < 3; i++)
        {
            int randomPick = RanomChecking(i);
            tmpCheckArray[i] = randomPick;
        }
       

        // 랜덤 적용된 퍽들 적용 시키기
        foreach (var p in _pickPerks) 
        {
            int number = tmpCheckArray[count];                                   // 퍽 인덱스 랜덤 번호 값.

            Image image = p.transform.GetChild(1).GetComponent<Image>();         // 이미지
            Text name = p.transform.GetChild(2).GetComponent<Text>();            // 퍽 이름
            Text level = p.transform.GetChild(3).GetComponent<Text>();           // 퍽 레벨
            Text ex = p.transform.GetChild(4).GetComponent<Text>();              // 퍽 설명

            image.sprite = PerkM.GetGamePerks[number].PerkSprite;
            name.text = PerkM.GetGamePerks[number].PerkTooltip;
            level.text = PerkM.GetGamePerks[number].PerkEffect.StringToLevel();
            ex.text =  PerkM.GetGamePerks[number].PerkEffect.StringToPickToolTip();

            // 만들어진 퍽에 현재 넣어진 퍽 띄우기
            HoverPerk hoverPerk = _pickPerks[count].GetComponent<HoverPerk>();
            hoverPerk.ReturnPerk = PerkM.GetGamePerks[number];                  // 퍽 넣어주기

            _pickPerks[count].gameObject.SetActive(true);                       // 퍽 UI 키기
            count++;
        }



        int RanomChecking(int idx)
        {
            int randNum;
            while (true)
            {
                randNum = Random.Range(0, PerkM.GetGamePerks.Length);            // 현재 퍽 길이까지 랜덤으로 숫자 뽑기
                bool isDupleCheck = false;


                // 첫 숫자는 넣기
                if (idx == 0) break;

                // 첫 숫자 이후 반복 검사
                for(int i = 0; i < tmpCheckArray.Length; i++)
                {
                    if (tmpCheckArray[i] == randNum)
                    {
                        isDupleCheck = true;
                        break;
                    }
                }

                // 반복문 조건에 걸리지 않았다면 return 해주기
                if (!isDupleCheck) break;
            }

            return randNum;
        }
    }

}
