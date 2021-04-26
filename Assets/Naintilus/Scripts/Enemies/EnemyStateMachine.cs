public class EnemyStateMachine<T> where T : EnemyBehaviour
{
    public EnemyState<T> CurrentState { get; private set; }

    public void Initialize(EnemyState<T> startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState<T> newState)
    {
        CurrentState.Exit();

        CurrentState = newState;
        CurrentState.Enter();
    }
}
