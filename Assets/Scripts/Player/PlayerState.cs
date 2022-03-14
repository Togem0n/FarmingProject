using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    // protected : similar to private, except class inherited from this script can use it
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;

    protected float startTime;

    private string animBoolName;

    // constructor
    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    // virtual can be overidden by classes that inheriented from this class;
    public virtual void Enter()
    {
        DoChecks();
        player.animator.SetBool(animBoolName, true);
        startTime = Time.time;
        //Debug.Log(animBoolName);

        isAnimationFinished = false;
    }

    public virtual void Exit()
    {
        player.animator.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }

    public virtual void AnimationTrigger()
    {

    }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;

}