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

        HoeStateCheck();

        CarrySeedStateCheck();

        UseToolStateCheck();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void HoeStateCheck()
    {
        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.HoeingTool
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);

            if (GridDetailsManager.Instance.IsHoeable(player.useToolGridPosition.x, player.useToolGridPosition.y))
            {
                stateMachine.ChangeState(player.HoeingState);
            }

        }
    }

    public void CarrySeedStateCheck()
    {
        if (InventoryManager.Instance.SelectedItemCode != -1
            && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.Seed
            && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
            && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetPlantDirection(Input.mousePosition.x, Input.mousePosition.y);
            stateMachine.ChangeState(player.CarryingSeedState);
        }
    }

    public void WateringStateCheck()
    {
        if (InventoryManager.Instance.SelectedItemCode != -1
    && InventoryManager.Instance.GetSelectedItemDetails().itemType == ItemType.WateringTool
    && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
    && !UIManager.Instance.IsPointerOverUIElement())
        {
            player.SetUseToolDirection(Input.mousePosition.x, Input.mousePosition.y);
            stateMachine.ChangeState(player.WateringState);
        }
    }

    public void UseToolStateCheck()
    {
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

}
