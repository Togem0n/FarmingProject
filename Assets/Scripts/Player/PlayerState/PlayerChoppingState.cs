using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoppingState : PlayerUseToolState
{
    public PlayerChoppingState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

        Crop crop = GridDetailsManager.Instance.GetCropObjectAtGridLocation(gridDetails);
        if (crop != null)
        {
            crop.ProcessToolAction(InventoryManager.Instance.GetSelectedItemDetails());
        }
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
