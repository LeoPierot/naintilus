using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyChaseState : EnemyState<JellyfishBehaviour>
{
    private float _moveSpeed;

    public JellyChaseState(JellyfishBehaviour jellyfish, EnemyStateMachine<JellyfishBehaviour> stateMachine)
        : base(jellyfish, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Enter Chase State");

        _moveSpeed = _enemy.ChaseSpeed;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        if (_enemy.Target != null)
        {
            _enemy.SetDirection(_enemy.Target.position - _enemy.transform.position);
        }

        _enemy.Move(_moveSpeed);
    }
}
