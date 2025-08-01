using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PlayerBasicUI : MonoBehaviour
{
    // Character ��������
    private CharacterHP _characterHP;

    // HP
    [SerializeField, Tooltip("HP ��������Ʈ")] private List<Image> healthImage = new List<Image>();

    // Stamina
    [SerializeField, Tooltip("Stamina ��������Ʈ")] private Image staminaImage;

    // QuickList
    [SerializeField, Tooltip("QuickTrans")] private Transform _quickTransform;
    private List<Image> _quickMainImage = new List<Image>();

    // BuffSlotList
    [SerializeField, Tooltip("BuffSlot")] private Transform _buffTransform;

    private List<GameObject> _buffObjects = new List<GameObject>();
    private List<Image> _buffImage = new List<Image>();
    private List<Image> _buffBackDelayImage = new List<Image>();

    #region ������Ƽ
    public PlayerBuffBuffer GetBuffer { get { return PlayerManager.Instance.GetPlayer.GetBuffBuffer; } }
    private float curHp { get { return _characterHP.currentHP; } }
    private float maxHP { get { return _characterHP.maxHP; } }
    private float curStamina { get { return _characterHP.currentStamina; } }
    private float maxStamina { get { return _characterHP.maxStamina; } }

    public float RecoverStatsHealTick(float curLevel)
    {
        float tick = Mathf.Lerp(1.0f, 3.33f, curLevel / 5f); // 1�� �� 3.33��
        return tick;
    }
    #endregion

    #region     ����
    // Amount����
    private float Amount(float cur, float max) => cur / max;
    // Staminȸ�� ����
    private float StaminaHeal() => 1.5f * RecoverStatsHealTick(UIManager.Instance.GetPlayerUI.GetStaminaPuckLevel) * Time.deltaTime;           // 1.5f ���̽�
    #endregion

    #region HP ����
    private bool isMovingHealth = false;

    public void InitHealth(CharacterHP script)
    {
        _characterHP = script;

        foreach (Image image in healthImage)            // ü�� �̹��� ui �ʱ�ȭ
            image.fillAmount = Amount(curHp, maxHP);

        staminaImage.fillAmount = Amount(curStamina, maxStamina);
    }

    public void UITakeDamage()
    {
        healthImage[0].fillAmount = Amount(curHp, maxHP);
        if (!isMovingHealth) { isMovingHealth = true; StartCoroutine(UpdateAnimHealth()); }
    }

    public void UIGetHeal()
    {
        foreach (Image image in healthImage)
            image.fillAmount = Amount(curHp, maxHP);
    }

    IEnumerator UpdateAnimHealth()
    {
        float velocity = 0f;
        float speed = 0.9f;

        yield return DelayForCode(0.5f);

        while (true)
        {
            if (healthImage[1].fillAmount <= healthImage[0].fillAmount + 0.001f) break;

            healthImage[1].fillAmount =
                Mathf.SmoothDamp(healthImage[1].fillAmount, healthImage[0].fillAmount, ref velocity, speed);

            yield return null;
        }

        healthImage[1].fillAmount = healthImage[0].fillAmount;
        isMovingHealth = false;
        yield return null;
    }

    #endregion

    #region ���¹̳� ����
    public void UseStaminaUI()
    {
        staminaImage.fillAmount = Amount(curStamina, maxStamina);
    }
   
    private void RecoverStamina()
    {
        if (curStamina < maxStamina)
        {
            // ������ ��� �� 3�ʰ� ������ ȸ��
            if (Time.time - _characterHP.lastUseTime >= _characterHP.recoveryDelay)
            {
                _characterHP.currentStamina += StaminaHeal();
                if (curStamina >= maxStamina) _characterHP.currentStamina = maxStamina;
            }
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaImage.fillAmount != 1.0f)
        {
            staminaImage.fillAmount = Amount(curStamina, maxStamina);
        }
    }
    #endregion

    #region QuickSlot Bar �� BuffList UI ����
    public void InitComponent()
    {
        for (int i = 0; i < _quickTransform.childCount; i++)
        {
            Image image = _quickTransform.GetChild(i).GetChild(0).GetComponent<Image>();
            _quickMainImage.Add(image);
        }

        for(int i = 0; i < _buffTransform.childCount; i++)                          // ���� ������
        {
            GameObject obj = _buffTransform.GetChild(i).gameObject;
            Image image = obj.transform.GetChild(0).GetComponent<Image>();  
            Image backImage = obj.transform.GetChild(1).GetComponent<Image>();

            _buffObjects.Add(obj);
            _buffImage.Add(image);
            _buffBackDelayImage.Add(backImage);
        }
    }
    public void MainQuickUpdateUI(Sprite sprite,int index, bool active = true)
    {
        int alpha = (active) ? 1 : 0;

        _quickMainImage[index].sprite = (active) ? sprite : null;           // active�� false�� null
        _quickMainImage[index].color = new Color(1, 1, 1, alpha);
    }

    public void MainBuffUpdateUI()
    {
        if (!GetBuffer.ListBuffBuffer.Any()) return;

        var buffs = GetBuffer.ListBuffBuffer;
        int checkSlot = _buffTransform.childCount;

        for(int i = 0; i < checkSlot; i++)
        {
            if (i >= buffs.Count()) RemoveBuffSprite(i);
        }

        for (int i = 0; i < buffs.Count; i++)
        {
            if (!_buffObjects[i].activeSelf) _buffObjects[i].SetActive(true);
            if (_buffImage[i].sprite == null) _buffImage[i].sprite = buffs[i].inBuffer.SkillSprite;

            float duration = buffs[i].duration;
            float Alltime = buffs[i].inBuffer.SkillBuffDuration;

            float progress = Mathf.Clamp01(1f - (duration / Alltime));

            _buffBackDelayImage[i].fillAmount = progress;
        }

    }


    public void RemoveBuffSprite(int idx)
    {
        if (!_buffObjects[idx].activeSelf) return;

        _buffObjects[idx].gameObject.SetActive(false);
        _buffImage[idx].sprite = null;
        _buffBackDelayImage[idx].fillAmount = 0;
    }
    #endregion 

    void Awake()
    {
        InitComponent();
    }

    private void Update()                                   // Update�� ���� ���¹̳� ȸ��
    {
        RecoverStamina();
        UpdateStaminaUI();
        MainBuffUpdateUI();
    }
    private IEnumerator DelayForCode(float time)
    {
        float current = 0f;
        float percent = 0f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            yield return null;
        }
    }
}
