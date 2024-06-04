using UnityEngine;

public class MonsterViewModel : CharacterViewModel
{
    public float randomMoveInterval = 3f;
    private float randomMoveTimer = 0f;
    private Vector3 randomTarget;
    private Transform player;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        ChangeState(new IdleState(this));
    }

    protected override void Update()
    {
        base.Update();

        if (currentState is IdleState)
        {
            RandomMove();
        }
    }

    private void RandomMove()
    {
        randomMoveTimer -= Time.deltaTime;

        if (randomMoveTimer <= 0f)
        {
            randomMoveTimer = randomMoveInterval;
            randomTarget = GetRandomPosition();
            MoveTo(randomTarget);
        }
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = transform.position.x + Random.Range(-10f, 10f);
        float randomZ = transform.position.z + Random.Range(-10f, 10f);
        Vector3 randomPosition = new Vector3(randomX, transform.position.y, randomZ);

        return randomPosition;
    }


    protected override void Die()
    {
        Debug.Log("Monster died.");
        gameObject.SetActive(false);
    }
}
