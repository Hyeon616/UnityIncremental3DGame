using System.Collections.Generic;

public class PlayerBehaviorTree : Singleton<PlayerBehaviorTree>
{
    private Node root;
    private Dictionary<int, float> skillCooldowns = new Dictionary<int, float>();

    public PlayerBehaviorTree()
    {
        InitializeSkillCooldowns();
        ConstructBehaviorTree();
    }

    private void InitializeSkillCooldowns()
    {
        var playerSkills = GameLogic.Instance.PlayerSkills;

        foreach (var skill in playerSkills)
        {
            skillCooldowns[skill.skill_id] = 0f;
        }

    }

    private void ConstructBehaviorTree()
    {
        root = new SelectorNode(new List<Node>
        {
            new SequenceNode(new List<Node>
            {
                new CheckForTarget(),
                new SelectorNode(new List<Node>
                {
                    //new UseSkill(0),
                    //new UseSkill(1),
                    //new UseSkill(2),
                    new BasicAttack(),
                })
            }),
            new Idle()
        });

    }



    public NodeState Update(float deltaTime)
    {
        var playerSkills = GameLogic.Instance.PlayerSkills;
        foreach (var skill in playerSkills)
        {
            if (skillCooldowns[skill.skill_id] > 0)
            {
                skillCooldowns[skill.skill_id] -= deltaTime;
            }
        }
        return root.Evaluate();

    }

    private class CheckForTarget : Node
    {

        public override NodeState Evaluate()
        {
            return NodeState.Success;
        }

    }

    

    private class BasicAttack : Node
    {
        public override NodeState Evaluate()
        {
            return NodeState.Success;
        }
    }

    private class Idle : Node
    {
        public override NodeState Evaluate()
        {
            return NodeState.Running;
        }
    }

}
