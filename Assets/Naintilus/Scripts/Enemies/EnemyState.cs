public class EnemyState<T> where T : EnemyBehaviour
{
    protected T _enemy;
    protected EnemyStateMachine<T> _stateMachine;

    public EnemyState(T enemy, EnemyStateMachine<T> stateMachine)
    {
        _enemy = enemy;
        _stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicUpdate() { }
    public virtual void Exit() { }
}
