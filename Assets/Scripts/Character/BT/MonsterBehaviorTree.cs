using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterBehaviorTree
{
    private Node root;
    private MonsterController monsterController;

    public MonsterBehaviorTree(MonsterController controller)
    {
        this.monsterController = controller;
        ConstructBehaviorTree();
    }

    private void ConstructBehaviorTree()
    {
        root = new SelectorNode(new List<Node>
        {
            new SequenceNode(new List<Node>
            {
                new FindPlayer(monsterController),
                new MoveTowardsPlayer(monsterController),
                new AttackPlayer(monsterController)
            }),
            new Idle(monsterController)
        });
    }

    public NodeState Update(float deltaTime)
    {
        return root.Evaluate();
    }


    private class FindPlayer : Node
    {
        private MonsterController controller;
        public FindPlayer(MonsterController controller)
        {
            this.controller = controller;
        }
        public override NodeState Evaluate()
        {
            GameObject player = controller.FindPlayer();
            return player != null ? NodeState.Success : NodeState.Failure;
        }
    }

    private class MoveTowardsPlayer : Node
    {
        private MonsterController controller;
        public MoveTowardsPlayer(MonsterController controller)
        {
            this.controller = controller;
        }
        public override NodeState Evaluate()
        {
            if (controller.GetPlayerTransform() != null)
            {
                controller.MoveTowardsPlayer();
                return NodeState.Running;
            }
            return NodeState.Failure;
        }
    }


    private class AttackPlayer : Node
    {
        private MonsterController controller;
        public AttackPlayer(MonsterController controller)
        {
            this.controller = controller;
        }
        public override NodeState Evaluate()
        {
            if (controller.GetPlayerTransform() != null)
            {
                return controller.AttackPlayerIfInRange() ? NodeState.Success : NodeState.Running;
            }
            return NodeState.Failure;
        }
    }


    private class Idle : Node
    {
        private MonsterController controller;
        public Idle(MonsterController controller)
        {
            this.controller = controller;
        }
        public override NodeState Evaluate()
        {
            controller.Idle();
            return NodeState.Running;
        }
    }
}
