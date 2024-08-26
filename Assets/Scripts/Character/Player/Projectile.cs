using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 5f;
    private float maxLifetime = 10f; // 최대 생존 시간
    private int damage;
    private Action<Projectile> returnPool;
    private float lifetime;
    private PlayerModel playerModel;
    private UI_DamageText damageText;

    [SerializeField] private AudioClip launchSound;

    public void Initialize(int damage, Action<Projectile> returnAction, PlayerModel playerModel)
    {
        this.damage = damage;
        this.returnPool = returnAction;
        this.playerModel = playerModel;
        lifetime = 0f;
    }

    private void OnEnable()
    {
        lifetime = 0f;
        if (launchSound != null && AudioManager.Instance !=null)
        {
            AudioManager.Instance.PlaySound(launchSound, transform.position);
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
        {
            ReturnPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                float criticalChance = playerModel.attributes.critical_chance;
                float criticalDamage = playerModel.attributes.critical_damage;
                bool isCritical = UnityEngine.Random.value < criticalChance;
                int finalDamage = CombatManager.Instance.CalculateDamage(damage, criticalChance, criticalDamage);
                monster.TakeDamage(finalDamage);

                // 데미지 텍스트 생성
                CombatManager.Instance.ShowDamageText(finalDamage, isCritical, monster.transform.position);

                ReturnPool();
            }
        }
    }

    private void ReturnPool()
    {
        returnPool?.Invoke(this);
    }
}
