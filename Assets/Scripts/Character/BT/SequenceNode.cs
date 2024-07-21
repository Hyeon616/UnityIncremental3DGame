using System.Collections.Generic;

public class SequenceNode : Node
{

    private List<Node> nodes = new List<Node>();

    public SequenceNode(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        bool isNodeRunning = false;

        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    isNodeRunning = true;
                    continue;
                case NodeState.Success:
                    continue;
                case NodeState.Failure:
                    _state = NodeState.Failure;
                    return _state;
            }
        }

        _state = isNodeRunning ? NodeState.Running : NodeState.Success;
        return _state;

    }
}

