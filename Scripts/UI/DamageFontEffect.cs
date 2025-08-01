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

        // ���� �� �̸� ����
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


    private IEnumerator FontAlphaAnim()                                     // ��Ʈ�� ���İ�..
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
    /// �� �κ��� ���� ��ü�� gpt���� ���..
    /// ĵ���� ��ǥ�� ����Ƽ ��ǥ�� �����ϱ� ������� 
    /// </summary>
    /// <param name="enemyPos"></param>
    /// <returns></returns>
    private IEnumerator FontPosAnim(Vector3 enemyPos)                    // ��Ʈ�� ������
    {
        // ������ ��ġ ���� �����ϰ� ���ֱ� ����

        float ranX = Random.Range(-1f, 1f);
        float ranY = Random.Range(-0.5f, 0.5f);

        enemyPos += new Vector3(ranX, 2f + ranY, 0);

        Vector3 uiPosition = Camera.main.WorldToScreenPoint(enemyPos);         //gpt ��... ���� �� ��ġ�� ��ũ�� ��ǥ�� ��ȯ

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (_canvasRect, uiPosition, null, out localPos);  // ��ũ�� ��ǥ�� ĵ���� ���� ��ǥ�� ��ȯ ��Ŵ

        _rectTrans.anchoredPosition = localPos;                                 // ��ȯ�� ��ǥ�� ����� �̵�

        // ���� ��ġ �����ֱ�
        Vector2 startPos = localPos;                                                                               // ���� ��ġ ���� �ϰ�
        Vector2 endPos = localPos + new Vector2(0f, 20f);                                                             // Y������ 20f ����

        // 3. �ִϸ��̼� �ð� ����
        float duration = 1f;
        float elapsed = 0f;

        // 4. �̵� �ִϸ��̼� ����
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // �������� ��ġ �̵�
            _rectTrans.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        // 5. ���� ��ġ ���� (Ȥ�� �����Ǵ� frame ����)
        _rectTrans.anchoredPosition = endPos;
    }


    private void OnDisable()
    {
        UIManager.Instance.GetDMGPoolUI.SetPushPool(this.gameObject);       // Ǯ�� �ٽ� �ֱ�
        _outLine.enabled = false;                                           // �ƿ����� ����

        _rectTrans.localPosition = _pivotTrans;                             // ���� ��ġ�� �����ֱ�
        _text.color = _refColor;                                            // ���� ���İ����� ������ ����
    }
}
