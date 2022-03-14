using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{

    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(InventoryManager.Instance.SelectedItemCode != -1 
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.HoeingTool 
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F)))
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);
            // if position is allowed to hoeing
            stateMachine.ChangeState(player.HoeingState);
        }

        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.Seed
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F)))
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);
            // if position is allowed to plant
            stateMachine.ChangeState(player.PlantingState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
