using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetFloat("xInput", player.moveDirection.x);
        player.animator.SetFloat("yInput", player.moveDirection.y);
        player.animator.SetFloat("speed", 0);
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

        if(player.xInput !=0 || player.yInput != 0)
        {
            stateMachine.ChangeState(player.WalkState);
        }

    }
}
