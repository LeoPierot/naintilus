using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkPatrolState : EnemyState<SharkBehaviour>
{
    private float _moveSpeed;
    private float _reactionDistance;

    public SharkPatrolState(SharkBehaviour shark, EnemyStateMachine<SharkBehaviour> stateMachine) : base(shark, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        _moveSpeed = _enemy.RegularSpeed;
        _reactionDistance = _enemy.ReactionDistance;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Vector3.Distance(_enemy.transform.position, _enemy.Target.position) <= _reactionDistance)
        {
            _stateMachine.ChangeState(_enemy.alertState);
        }
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        _enemy.Move(_moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        _enemy.Move(0.0f);
    }
}
