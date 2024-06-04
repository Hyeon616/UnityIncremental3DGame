using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private int maxHealth = 100;
    private int currentHealth;
    private int attackPower = 6; // 임시 변수로 공격력 저장

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    void Die()
    {
        // TODO 사망 처리
        Debug.Log("Player die");
    }


}
