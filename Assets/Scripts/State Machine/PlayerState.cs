public abstract class PlayerState
{
    protected PlayerStateManager context;
    public PlayerState(PlayerStateManager context)
    {
        this.context = context;
    }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
