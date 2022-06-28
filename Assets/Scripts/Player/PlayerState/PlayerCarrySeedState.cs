using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarrySeedState : PlayerUseToolState
{
    public PlayerCarrySeedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

        if (gridDetails != null 
            && gridDetails.daysSinceDug > -1 
            && gridDetails.seedItemCode == -1
            )
        {
            gridDetails.seedItemCode = InventoryManager.Instance.SelectedItemCode;
            gridDetails.growthDays = 0;

            GridDetailsManager.Instance.DisplayPlantedCrop(gridDetails);

            // remove item;
            InventoryManager.Instance.RemoveSelectedItemByOne();
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
