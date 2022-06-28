using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoeingState : PlayerUseToolState
{
    public PlayerHoeingState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {

        player.animator.SetFloat("useToolDirectionX", player.useToolGridDirection.x);
        player.animator.SetFloat("useToolDirectionY", player.useToolGridDirection.y);
        player.animator.SetFloat("xInput", player.moveDirection.x);
        player.animator.SetFloat("yInput", player.moveDirection.y);

        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        GridDetailsManager.Instance.HoeingGround(player.useToolGridPosition.x, player.useToolGridPosition.y);
        stateMachine.ChangeState(player.IdleState);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }
}
