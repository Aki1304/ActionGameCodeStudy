using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAttackDelay
{
    private Player _player { get { return PlayerManager.Instance.GetPlayer; } }

    private bool wasMoving = false;
    private float moveStartTimer = 0f;
    private const float attackDelayAfterMove = 0.18f;

    public void DelayForClick()
    {
        bool isNowMoving = _player.IsMoving;

        // ó������ �����̱� ������ ����
        if (isNowMoving && !wasMoving)
        {
            moveStartTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        }

        // �����̴� ���� �ð� ����
        if (isNowMoving)
        {
            moveStartTimer += Time.deltaTime;
        }

        wasMoving = isNowMoving; // ���� ����
    }

    public bool CanAttackAfterMoveDelay()
    {
        if (!_player.IsMoving) return true;

        // ���� ������ ���� ���ų�, ������ �� 0.18�� �̻� ������ ���� ���� ���
        return moveStartTimer >= attackDelayAfterMove;
    }
}
