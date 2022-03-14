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
        base.Enter();

        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);
        
        if(gridPropertyDetails != null && gridPropertyDetails.daysSinceDug == -1)
        {

            gridPropertyDetails.daysSinceDug = 0;
            
            GridPropertyManager.Instance.SetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y, gridPropertyDetails);

            //GridPropertyManager.Instance.SetTileToDug(player.useToolGridPosition.x, player.useToolGridPosition.y);
            GridPropertyManager.Instance.DisplayDugGround(gridPropertyDetails);
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

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        stateMachine.ChangeState(player.IdleState);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }
}
