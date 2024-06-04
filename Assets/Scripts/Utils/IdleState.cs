public class IdleState : ICharacterState
{
    private readonly CharacterViewModel character;

    public IdleState(CharacterViewModel character)
    {
        this.character = character;
    }

    public void Enter()
    {
        character.characterView.Animator.SetBool("isWalking", false);
        character.characterView.Animator.SetBool("isAttacking", false);
    }

    public void Execute()
    {
        // 적을 찾기
        character.FindTarget();
    }

    public void Exit()
    {
        // Idle 상태에서 나갈 때 할 일
    }
}
