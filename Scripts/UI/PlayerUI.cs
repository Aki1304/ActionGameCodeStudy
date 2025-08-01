using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    #region 객체 가져오기
    [SerializeField] private PlayerBasicUI _playerBasicUI; 

    #endregion

    #region 컴포넌트

    [Header("걷기 뛰기 아이콘")]
    [SerializeField] private List<Sprite> iconWRSprites = new List<Sprite>();

    [Header("걷기 IconTrans")]
    [SerializeField] private Image iconWRImage;
    public Sprite GetWRSprite {  get { return iconWRImage.sprite; } set { iconWRImage.sprite = value; } }

    #endregion

    #region 걷기 뛰기 표시
    public Sprite GetReturnWRSprite(bool active)
    {
        int numberIcon = active ? 1 : 0;

        // 0 걷기 1 뛰기
        return iconWRSprites[numberIcon];
    }

    public void UICharacterInit()
    {
        // Alpha 값 0 > 1 변경 해주기

        iconWRImage.color = Color.white;
        GetWRSprite = GetReturnWRSprite(false);
    }

    #endregion 

    #region 각 기능하는 객체에게 정보 건네주기
    public void InitPlayerUI (CharacterHP characterHP)
    {
        _playerBasicUI.InitHealth(characterHP);
    }
    public void PassHealInfo() => _playerBasicUI.UIGetHeal();
    public void PassDamageInfo() => _playerBasicUI.UITakeDamage();
    public void PassStaminaInfo() => _playerBasicUI.UseStaminaUI();
    public void PassMainQuickSprite(Sprite sprite, int index, bool active = true) => _playerBasicUI.MainQuickUpdateUI(sprite, index, active);
    public void GetRemoveBufferIndex(int index) => _playerBasicUI.RemoveBuffSprite(index);

    #endregion

    #region 퍽 스태미나 회복 틱 레벨
    public int GetStaminaPuckLevel = 0;
    #endregion

}
