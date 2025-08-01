using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    #region ��ü ��������
    [SerializeField] private PlayerBasicUI _playerBasicUI; 

    #endregion

    #region ������Ʈ

    [Header("�ȱ� �ٱ� ������")]
    [SerializeField] private List<Sprite> iconWRSprites = new List<Sprite>();

    [Header("�ȱ� IconTrans")]
    [SerializeField] private Image iconWRImage;
    public Sprite GetWRSprite {  get { return iconWRImage.sprite; } set { iconWRImage.sprite = value; } }

    #endregion

    #region �ȱ� �ٱ� ǥ��
    public Sprite GetReturnWRSprite(bool active)
    {
        int numberIcon = active ? 1 : 0;

        // 0 �ȱ� 1 �ٱ�
        return iconWRSprites[numberIcon];
    }

    public void UICharacterInit()
    {
        // Alpha �� 0 > 1 ���� ���ֱ�

        iconWRImage.color = Color.white;
        GetWRSprite = GetReturnWRSprite(false);
    }

    #endregion 

    #region �� ����ϴ� ��ü���� ���� �ǳ��ֱ�
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

    #region �� ���¹̳� ȸ�� ƽ ����
    public int GetStaminaPuckLevel = 0;
    #endregion

}
