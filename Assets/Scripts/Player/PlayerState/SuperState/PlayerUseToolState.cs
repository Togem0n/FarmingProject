using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseToolState : PlayerState
{
    public PlayerUseToolState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetFloat("speed", 0);
        player.DisablePlayerInput();
        // enable when animation finish
    }

    public override void Exit()
    {
        base.Exit();
        player.EnablePlayerInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
