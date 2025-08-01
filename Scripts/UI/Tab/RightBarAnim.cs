using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightBarAnim : MonoBehaviour
{
    #region ������Ƽ
    public bool GetAnimPlaying { get; set; }    // ���� �Ǻ�
    public bool GetStateBar { get; set; }       // true ���� false �ݱ�
    public bool GetCheckAnim() { return !GetAnimPlaying; }
    #endregion

    public void SetPlaying(bool active)
    {
        GetAnimPlaying = active;
    }


    public IEnumerator MoveBar(RectTransform rectTrans)
    {
        Vector2 firstPos = new Vector2(64f,146.5f);
        Vector2 secondPos = new Vector2(-158.5f,146.5f);

        Vector2 targetPos = (rectTrans.anchoredPosition == firstPos) ? secondPos : firstPos;
        GetStateBar = (targetPos == secondPos);

        Close();

        float speed = Time.deltaTime * 800;

        while(rectTrans.anchoredPosition != targetPos)
        {
            rectTrans.anchoredPosition =
                Vector2.MoveTowards(rectTrans.anchoredPosition, targetPos, speed);

            yield return null;
        }

        SetPlaying(false);
        yield return null;
    }

    private void Close()
    {
        if (GetStateBar) return; else UIManager.Instance.OepnCloseUI();
    }

}
