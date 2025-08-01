using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    private RectTransform _rect;            // 현재 ui의 렉트

    private GameObject _childImageObject;   // 텍스트 오브젝트 처음에 꺼두기
    private RectTransform _textRect;        // 텍스트 ui의 렉트
    private Text _text;                     // 텍스트
    private ContentSizeFitter _conSize;     // 콘텐츠 사이즈

    private Vector2 _defaultSize;             // 기존 크기

    private bool _anyState = false;         // 기본 불 값 체크 용도

    #region 프로퍼티
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
        if (_anyState)                              // 일반 사용
        {
            if (Input.anyKeyDown) DisableWindowUI();
        }
    }

    #region window 동작 부분
    // 공통 사용
    private void RectSizeChange() => _rect.sizeDelta = new Vector2((_rect.sizeDelta.x + _textRect.sizeDelta.x), (_rect.sizeDelta.y + _textRect.sizeDelta.y));

    /// <summary>
    /// 일반 적인 WindowUI는 이것을 사용
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
