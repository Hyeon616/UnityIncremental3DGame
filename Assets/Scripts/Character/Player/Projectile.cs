using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 5f;
    private float maxLifetime = 5f; // 최대 생존 시간
    private int damage;
    private Action<Projectile> returnToPool;
    private float lifetime;

    public void Initialize(int damage, Action<Projectile> returnAction)
    {
        this.damage = damage;
        this.returnToPool = returnAction;
        lifetime = 0f;
    }

    private void OnEnable()
    {
        lifetime = 0f;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        returnToPool?.Invoke(this);
    }
}
