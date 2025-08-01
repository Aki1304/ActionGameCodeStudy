using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class DamageFontEffect : MonoBehaviour
{
    public RectTransform _canvasRect;
    private RectTransform _rectTrans;
    private Outline _outLine;

    private Color _refColor;

    private Text _text;

    private Vector3 _pivotTrans;

    private int curDamage;

    public void InitClass(Canvas canvas)
    {
        _canvasRect = canvas.GetComponent<RectTransform>();
        _rectTrans = GetComponent<RectTransform>();
        _text = GetComponent<Text>();
        _outLine = GetComponent<Outline>();

        // 기준 색 미리 지정
        _pivotTrans = _rectTrans.localPosition;
        _refColor = _text.color;
    }

    public void SetActiveTrue(int damage, Vector3 enemyPos, bool criticalCheck = false)
    {
        curDamage = damage;
        if(criticalCheck) _outLine.enabled = true;

        this.gameObject.SetActive(true);

        StartCoroutine(FontAlphaAnim());    
        StartCoroutine(FontPosAnim(enemyPos));
    }


    private IEnumerator FontAlphaAnim()                                     // 폰트의 알파값..
    {
        Color curColor = _text.color;

        _text.text = curDamage.ToString();

        while(curColor.a > 0f)
        {
            curColor.a -= Time.deltaTime;

            curColor.a = Mathf.Clamp01(curColor.a);

            _text.color = curColor;            

            yield return null;
        }

        this.gameObject.SetActive(false);

        yield return null;
    }


    /// <summary>
    /// 이 부분은 공식 자체를 gpt한테 물어봄..
    /// 캔버스 좌표와 유니티 좌표를 구분하기 어려웠음 
    /// </summary>
    /// <param name="enemyPos"></param>
    /// <returns></returns>
    private IEnumerator FontPosAnim(Vector3 enemyPos)                    // 폰트의 움직임
    {
        // 나오는 위치 조금 랜덤하게 해주기 위해

        float ranX = Random.Range(-1f, 1f);
        float ranY = Random.Range(-0.5f, 0.5f);

        enemyPos += new Vector3(ranX, 2f + ranY, 0);

        Vector3 uiPosition = Camera.main.WorldToScreenPoint(enemyPos);         //gpt 왈... 지금 적 위치를 스크린 좌표로 변환

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (_canvasRect, uiPosition, null, out localPos);  // 스크린 좌표를 캔버스 로컬 좌표로 변환 시킴

        _rectTrans.anchoredPosition = localPos;                                 // 변환한 좌표를 여기로 이동

        // 랜덤 위치 정해주기
        Vector2 startPos = localPos;                                                                               // 시작 위치 고정 하고
        Vector2 endPos = localPos + new Vector2(0f, 20f);                                                             // Y축으로 20f 위로

        // 3. 애니메이션 시간 설정
        float duration = 1f;
        float elapsed = 0f;

        // 4. 이동 애니메이션 루프
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 보간으로 위치 이동
            _rectTrans.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        // 5. 최종 위치 고정 (혹시 누락되는 frame 방지)
        _rectTrans.anchoredPosition = endPos;
    }


    private void OnDisable()
    {
        UIManager.Instance.GetDMGPoolUI.SetPushPool(this.gameObject);       // 풀에 다시 넣기
        _outLine.enabled = false;                                           // 아웃라인 끄기

        _rectTrans.localPosition = _pivotTrans;                             // 기준 위치로 돌려주기
        _text.color = _refColor;                                            // 기준 알파값으로 돌리는 역할
    }
}
