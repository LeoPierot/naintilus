using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishBehaviour : EnemyBehaviour
{
    public JellyPatrolState patrolState;
    public JellyAlertState alertState;
    public JellyChaseState chaseState;

    private EnemyStateMachine<JellyfishBehaviour> _jellyfishSM;

    private void Awake()
    {
        _jellyfishSM = new EnemyStateMachine<JellyfishBehaviour>();

        patrolState = new JellyPatrolState(this, _jellyfishSM);
        alertState = new JellyAlertState(this, _jellyfishSM);
        chaseState = new JellyChaseState(this, _jellyfishSM);

        _jellyfishSM.Initialize(patrolState);
    }

    public override void Start()
    {
        base.Start();

        SetDirection(Vector3.up);
    }

    public void Update()
    {
        _jellyfishSM.CurrentState.LogicUpdate();
    }

    public void FixedUpdate()
    {
        _jellyfishSM.CurrentState.PhysicUpdate();
    }

    public override void SetDirection(Vector3 direction)
    {
        base.SetDirection(direction);
    }

    public override void Move(float speed)
    {
        base.Move(speed);
    }
}
