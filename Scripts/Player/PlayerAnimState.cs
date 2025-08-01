using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimState
{
    #region ±âº» °È±â ¶Ù±â State
    public enum AnimState
    {
        Idle, Walk, Run
    }

    private AnimState playerState = AnimState.Idle;
    public AnimState GetPlayerState {  get { return playerState; } }

    public void SetState(AnimState state) => playerState = state;
    #endregion

    #region ±âº» °ø°Ý State
    public enum NormalAttackAnimState
    {
        Attack1, Attack2, Attack3
    }

    private NormalAttackAnimState playerNAState = NormalAttackAnimState.Attack1;
    public NormalAttackAnimState GetNormalAttackState { get { return playerNAState; } }
    public void SetNAState(NormalAttackAnimState state) => playerNAState = state;
    #endregion

    #region AnyState
    public enum AnyState
    {
        Hit, Die
    }
    #endregion
}
