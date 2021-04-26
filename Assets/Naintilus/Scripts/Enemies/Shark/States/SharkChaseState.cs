using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkChaseState : EnemyState<SharkBehaviour>
{
    private float _moveSpeed;

    public SharkChaseState(SharkBehaviour shark, EnemyStateMachine<SharkBehaviour> stateMachine) : base(shark, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Enter Chase State");

        _moveSpeed = _enemy.ChaseSpeed;

        if (_enemy.Target != null)
        {
            _enemy.SetDirection(_enemy.Target.position - _enemy.transform.position);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        _enemy.Move(_moveSpeed);
    }
}
