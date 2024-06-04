using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private int maxHealth = 100;
    private int currentHealth;
    private int attackPower = 6; // �ӽ� ������ ���ݷ� ����

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
        // TODO ��� ó��
        Debug.Log("Player die");
    }


}
