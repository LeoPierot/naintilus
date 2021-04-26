using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyAlertState : EnemyState<JellyfishBehaviour>
{
    private float _reactionTime;
    private float _reactionEndTime;

    public JellyAlertState(JellyfishBehaviour jellyfish, EnemyStateMachine<JellyfishBehaviour> stateMachine)
        : base(jellyfish, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        // play animation

        Debug.Log("Enter Alert State");

        _reactionTime = _enemy.ReactionTime;
        _reactionEndTime = Time.time + _reactionTime;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= _reactionEndTime)
        {
            _stateMachine.ChangeState(_enemy.chaseState);
        }
    }
}
