using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    protected List<Node> nodes = new List<Node>();

    public SelectorNode(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    _state = NodeState.Running;
                    return _state;
                case NodeState.Success:
                    _state = NodeState.Success;
                    return _state;
                case NodeState.Failure:
                    continue;
            }
        }

        _state = NodeState.Failure;
        return _state;

    }
}
