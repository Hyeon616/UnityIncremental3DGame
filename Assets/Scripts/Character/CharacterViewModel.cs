using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterViewModel : MonoBehaviour
{
    [SerializeField] private CharacterView CharacterView_CharacterView;
    [SerializeField] private NavMeshAgent NavMeshAgent_Agent;
    [SerializeField] private Transform Transform_Target;
    [SerializeField] private CharacterModel CharacterModel_CharacterModel;
    [SerializeField] private GameObject GameObject_DamageTextPrefab;

    private ICharacterState _currentState;
    private float _attackTimer;

    public ICharacterState CurrentState => _currentState;
    public CharacterView CharacterView => CharacterView_CharacterView;
    public NavMeshAgent Agent => NavMeshAgent_Agent;
    public Transform Target
    {
        get => Transform_Target;
        set => Transform_Target = value;
    }

    public CharacterModel CharacterModel => CharacterModel_CharacterModel;
    public float AttackTimer
    {
        get => _attackTimer;
        set => _attackTimer = value;
    }

    private void Awake()
    {
        if (CharacterModel_CharacterModel == null)
        {
            CharacterModel_CharacterModel = GetComponent<CharacterModel>();
            if (CharacterModel_CharacterModel == null)
            {
                Debug.LogError("CharacterModel_CharacterModel is not set and could not be found on the GameObject.");
            }
        }
    }

    public void SetState(ICharacterState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }

    public void ChangeState(ICharacterState newState)
    {
        SetState(newState);
    }

    public virtual void Update()
    {
        _currentState?.Execute();
    }

    protected abstract void Die();

    public Transform FindTarget(float detectionRange, string targetTag)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.transform;
                }
            }
        }

        return closestTarget;
    }

    public void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CharacterModel_CharacterModel.AttackRange);
        int targetsHit = 0;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(CharacterModel_CharacterModel.AttackPower);
                    Debug.Log(hitCollider.transform);
                    Debug.Log(CharacterModel_CharacterModel.AttackPower);
                    ShowDamage(hitCollider.transform, CharacterModel_CharacterModel.AttackPower);
                    targetsHit++;
                    if (targetsHit >= 2) break;
                }
            }
        }

        Debug.Log("ApplyDamage called");
    }

    protected virtual void ShowDamage(Transform targetTransform, int damageAmount)
    {
        if (GameObject_DamageTextPrefab == null)
        {
            Debug.LogError("GameObject_DamageTextPrefab is not set.");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
            return;
        }

        GameObject damageText = Instantiate(GameObject_DamageTextPrefab, canvas.transform);
        damageText.transform.position = Camera.main.WorldToScreenPoint(targetTransform.position);

        DamageText damageTextComponent = damageText.GetComponent<DamageText>();
        if (damageTextComponent == null)
        {
            Debug.LogError("DamageText component is not found on the damageText prefab.");
            return;
        }

        damageTextComponent.Setup(damageAmount);
    }





    public void OnAttackAnimationEnd()
    {
        if (_currentState is AttackingState)
        {
            _currentState.Exit();
            SetState(new IdleState(this, 100f, CharacterModel_CharacterModel.AttackRange));
        }
    }
}
