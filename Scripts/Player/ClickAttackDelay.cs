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

        // 처음으로 움직이기 시작한 순간
        if (isNowMoving && !wasMoving)
        {
            moveStartTimer = 0f; // 타이머 초기화
        }

        // 움직이는 동안 시간 누적
        if (isNowMoving)
        {
            moveStartTimer += Time.deltaTime;
        }

        wasMoving = isNowMoving; // 상태 갱신
    }

    public bool CanAttackAfterMoveDelay()
    {
        if (!_player.IsMoving) return true;

        // 아직 움직인 적이 없거나, 움직인 뒤 0.18초 이상 지났을 때만 공격 허용
        return moveStartTimer >= attackDelayAfterMove;
    }
}
