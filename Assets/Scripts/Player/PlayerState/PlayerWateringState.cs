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
        base.Enter();

        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

        if (gridPropertyDetails != null
            && gridPropertyDetails.daysSinceDug > -1
            && gridPropertyDetails.seedItemCode == -1
            )
        {
            gridPropertyDetails.daysSinceWatered = 1;

            GridPropertyManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
        }

        stateMachine.ChangeState(player.IdleState);
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
