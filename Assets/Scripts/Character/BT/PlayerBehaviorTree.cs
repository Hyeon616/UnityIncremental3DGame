using System.Collections.Generic;

public class PlayerBehaviorTree : Singleton<PlayerBehaviorTree>
{
    private Node root;
    private PlayerController playerController;
    public PlayerBehaviorTree(PlayerController controller)
    {
        this.playerController = controller;
        ConstructBehaviorTree();
    }

   

    private void ConstructBehaviorTree()
    {
        root = new SelectorNode(new List<Node>
        {
            new SequenceNode(new List<Node>
            {
                 new FindNearestMonster(playerController),
                new MoveTowardsMonster(playerController),
                new AttackMonster(playerController)
            }),
            new Idle(playerController)
        });

    }



    public NodeState Update(float deltaTime)
    {
        return root.Evaluate();

    }

    private class FindNearestMonster : Node
    {

        private PlayerController controller;

        public FindNearestMonster(PlayerController controller)
        {
            this.controller = controller;
        }

        public override NodeState Evaluate()
        {
            controller.NearestMonster = controller.FindNearestMonster();
            return controller.NearestMonster != null ? NodeState.Success : NodeState.Failure;
        }

    }

    

    private class MoveTowardsMonster : Node
    {
        private PlayerController controller;

        public MoveTowardsMonster(PlayerController controller)
        {
            this.controller = controller;
        }

        public override NodeState Evaluate()
        {
            if (controller.NearestMonster != null)
            {
                try
                {
                    controller.MoveTowardsMonster(controller.NearestMonster);
                    return NodeState.Running;
                }
                catch
                {
                    return NodeState.Failure;
                }
            }
            return NodeState.Failure;
        }
    }

    private class AttackMonster : Node
    {
        private PlayerController controller;

        public AttackMonster(PlayerController controller)
        {
            this.controller = controller;
        }

        public override NodeState Evaluate()
        {
            if (controller.NearestMonster != null && controller.NearestMonster.activeInHierarchy)
            {
                return controller.AttackMonsterIfInRange(controller.NearestMonster) ? NodeState.Success : NodeState.Running;
            }
            return NodeState.Failure;
        }
    }

    private class Idle : Node
    {
        private PlayerController controller;

        public Idle(PlayerController controller)
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
