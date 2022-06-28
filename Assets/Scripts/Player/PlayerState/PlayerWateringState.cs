using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWateringState : PlayerUseToolState
{
    public PlayerWateringState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        stateMachine.ChangeState(player.IdleState);
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
        player.animator.SetFloat("useToolDirectionX", player.useToolGridDirection.x);
        player.animator.SetFloat("useToolDirectionY", player.useToolGridDirection.y);
        player.animator.SetFloat("xInput", player.moveDirection.x);
        player.animator.SetFloat("yInput", player.moveDirection.y);

        base.Enter();



        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

        if (gridDetails != null
            && gridDetails.daysSinceDug > -1
            )
        {
            gridDetails.daysSinceWatered = 1;
            GridDetailsManager.Instance.DisplayPlantedCrop(gridDetails);
        }
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
}
