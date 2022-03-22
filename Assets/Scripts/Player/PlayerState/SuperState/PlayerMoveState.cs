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

        // dug logic
        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.HoeingTool
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);

            //TODO make if position is allowed to hoeing into a method
            GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

            if (gridPropertyDetails != null && gridPropertyDetails.daysSinceDug == -1 && gridPropertyDetails.seedItemCode == -1)
            {
                gridPropertyDetails.daysSinceDug = 0;

                GridPropertyManager.Instance.SetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y, gridPropertyDetails);

                GridPropertyManager.Instance.DisplayDugGround(gridPropertyDetails);

                stateMachine.ChangeState(player.HoeingState);
            }
            else
            {
                //Debug.Log("Not allowed to dug");
            }
        }

        // plant seed logic
        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.Seed
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetPlantDirection(Input.mousePosition.x, Input.mousePosition.y);
            // if position is allowed to plant
            stateMachine.ChangeState(player.CarryingSeedState);
        }

        // water crop logic
        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.WateringTool
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);
            // if position is allowed to plant
            Debug.Log("???");
            stateMachine.ChangeState(player.WateringState);
        }

        // harvest logic
        // change line 57 to
        // if InventoryManager.Instance.GetSelectedItemDetails().itemType == getcropdetails? if can harvest?
        if (InventoryManager.Instance.SelectedItemCode != -1
            && ((InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.CollectingTool
            || InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.ChoppingTool) 
            || InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.BreakingTool)
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement()
            && player.gridCursor.CursorPositionIsValid)
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);

            GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(player.useToolGridPosition.x, player.useToolGridPosition.y);

            Crop crop = GridPropertyManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

            if (crop != null)
            {
                switch (InventoryManager.Instance.GetSelectedItemDetails().itemType)
                {
                    case ItemType.BreakingTool:
                    case ItemType.ChoppingTool:
                        crop.ProcessToolAction(InventoryManager.Instance.GetSelectedItemDetails());
                        stateMachine.ChangeState(player.ChoppingState);
                        break;
                    case ItemType.CollectingTool:
                        crop.ProcessToolAction(InventoryManager.Instance.GetSelectedItemDetails());
                        stateMachine.ChangeState(player.HarvestingState);
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
