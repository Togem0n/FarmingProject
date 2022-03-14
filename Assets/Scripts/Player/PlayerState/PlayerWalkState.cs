using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerMoveState
{
    private int xInput;
    private int yInput;
    private float movementSpeed;
    private  Vector2 moveDirection;

    public PlayerWalkState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        movementSpeed = player.movementSpeed;
        moveDirection = player.moveDirection;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.xInput == 0 && player.yInput == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 move = new Vector2(player.xInput * player.movementSpeed * Time.deltaTime, player.yInput * player.movementSpeed * Time.deltaTime);

        player.rb.MovePosition(player.rb.position + move);

        player.animator.SetFloat("xInput", player.moveDirection.x);
        player.animator.SetFloat("yInput", player.moveDirection.y);
        player.animator.SetFloat("speed", move.normalized.magnitude);

    }
}
