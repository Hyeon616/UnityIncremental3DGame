

using Cysharp.Threading.Tasks;

public abstract class Node
{
    protected NodeState _state;

    public NodeState State { get => _state; }

    public abstract NodeState Evaluate();


}
