using UnityEngine;

public class PlayerViewModel : CharacterViewModel
{
    public void FullHeal()
    {
        CharacterModel.FullHeal();
    }
    protected override void Die()
    {
        Debug.Log("Player died.");
        // 추가적인 플레이어 사망 처리 로직
    }
}
