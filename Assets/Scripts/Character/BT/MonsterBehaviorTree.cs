using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
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
                new IsPlayerInRange(monsterController),
                new AttackPlayer(monsterController)
            }),
            new ChasePlayer(monsterController)
        });
    }

    public void Update()
    {
        root.Evaluate();
    }

    private class IsPlayerInRange : Node
    {
        private MonsterController controller;

        public IsPlayerInRange(MonsterController controller)
        {
            this.controller = controller;
        }

        public override NodeState Evaluate()
        {
            return controller.IsPlayerInRange() ? NodeState.Success : NodeState.Failure;
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
            controller.AttackPlayer();
            return NodeState.Success;
        }
    }

    private class ChasePlayer : Node
    {
        private MonsterController controller;

        public ChasePlayer(MonsterController controller)
        {
            this.controller = controller;
        }

        public override NodeState Evaluate()
        {
            controller.ChasePlayer();
            return NodeState.Running;
        }
    }
}
