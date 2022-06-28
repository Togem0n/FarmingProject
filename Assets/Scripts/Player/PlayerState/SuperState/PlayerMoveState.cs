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

        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.HoeingTool
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);

            //TODO make if position is allowed to hoeing into a method
            GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);
            
            if (gridDetails != null && gridDetails.daysSinceDug == -1 && gridDetails.seedItemCode == -1)
            {
                gridDetails.daysSinceDug = 0;

                GridDetailsManager.Instance.SetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y, gridDetails);

                GridDetailsManager.Instance.DisplayDugGround(gridDetails);

                stateMachine.ChangeState(player.HoeingState);
            }
            else
            {
                //Debug.Log("Not allowed to dug");
            }
        }

        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.Seed
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetPlantDirection(Input.mousePosition.x, Input.mousePosition.y);
            stateMachine.ChangeState(player.CarryingSeedState);
        }

        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.WateringTool
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);
            stateMachine.ChangeState(player.WateringState);
        }

        if (InventoryManager.Instance.SelectedItemCode != -1
            && ((InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.CollectingTool
            || InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.ChoppingTool) 
            || InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.BreakingTool)
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement()
            && player.gridCursor.CursorPositionIsValid)
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);


            GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

            Crop crop = GridDetailsManager.Instance.GetCropObjectAtGridLocation(gridDetails);
            if (crop != null)
            {
                switch (InventoryManager.Instance.GetSelectedItemDetails().itemType)
                {
                    case ItemType.CollectingTool:
                        stateMachine.ChangeState(player.HarvestingState);
                        break;
                    case ItemType.ChoppingTool:
                        stateMachine.ChangeState(player.ChoppingState);
                        break;
                    case ItemType.BreakingTool:
                        stateMachine.ChangeState(player.BreakingState);
                        break;
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
