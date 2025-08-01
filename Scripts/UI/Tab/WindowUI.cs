using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    private RectTransform _rect;            // ���� ui�� ��Ʈ

    private GameObject _childImageObject;   // �ؽ�Ʈ ������Ʈ ó���� ���α�
    private RectTransform _textRect;        // �ؽ�Ʈ ui�� ��Ʈ
    private Text _text;                     // �ؽ�Ʈ
    private ContentSizeFitter _conSize;     // ������ ������

    private Vector2 _defaultSize;             // ���� ũ��

    private bool _anyState = false;         // �⺻ �� �� üũ �뵵

    #region ������Ƽ
    private GameManager GM { get { return GameManager.Instance; } }
    #endregion

    public void DisableWindowUI() => this.gameObject.SetActive(false);

    private void OnEnable()
    {
        _anyState = true;
    }

    private void OnDisable()
    {
        _anyState = false;
        _rect.sizeDelta = _defaultSize;
    }

    private void Awake()
    {
        InitWindowUI();
    }

    private void Update()
    {

        KeyInput();
    }

    void InitWindowUI()
    {
        _rect = GetComponent<RectTransform>();
        _childImageObject = this.transform.GetChild(0).gameObject;
        _textRect = this.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        _text = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        _conSize = this.transform.GetChild(1).gameObject.GetComponent<ContentSizeFitter>();

        _defaultSize = _rect.sizeDelta;
        _childImageObject.gameObject.SetActive(true);
        DisableWindowUI();
    }


    void KeyInput()
    {
        if (_anyState)                              // �Ϲ� ���
        {
            if (Input.anyKeyDown) DisableWindowUI();
        }
    }

    #region window ���� �κ�
    // ���� ���
    private void RectSizeChange() => _rect.sizeDelta = new Vector2((_rect.sizeDelta.x + _textRect.sizeDelta.x), (_rect.sizeDelta.y + _textRect.sizeDelta.y));

    /// <summary>
    /// �Ϲ� ���� WindowUI�� �̰��� ���
    /// </summary>
    /// <param name="msg"></param>
    public void EnableWindowUI(string msg)
    {
        _text.text = msg;
        this.gameObject.SetActive(true);

        StartCoroutine(GetSizeChange());

        IEnumerator GetSizeChange()
        {
            yield return new WaitForEndOfFrame();
            RectSizeChange();
        }

    }

    #endregion
}
