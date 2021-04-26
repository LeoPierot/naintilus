using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBehaviour : EnemyBehaviour
{
    public SharkPatrolState patrolState;
    public SharkAlertState alertState;
    public SharkChaseState chaseState;

    private EnemyStateMachine<SharkBehaviour> _sharkSM;

    private void Awake()
    {
        _sharkSM = new EnemyStateMachine<SharkBehaviour>();

        patrolState = new SharkPatrolState(this, _sharkSM);
        alertState = new SharkAlertState(this, _sharkSM);
        chaseState = new SharkChaseState(this, _sharkSM);

        _sharkSM.Initialize(patrolState);
    }

    public override void Start()
    {
        base.Start();

        SetDirection(_target.position - transform.position);
    }

    public void Update()
    {
        _sharkSM.CurrentState.LogicUpdate();
    }

    public void FixedUpdate()
    {
        _sharkSM.CurrentState.PhysicUpdate();
    }

    public override void SetDirection(Vector3 direction)
    {
        base.SetDirection(direction);
        transform.right = direction;
    }

    public override void Move(float speed)
    {
        base.Move(speed);
    }
}
